using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PunkArchipelago;

[BepInProcess("Punk.exe")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[DefaultExecutionOrder(-1000)]
public class ArchipelagoPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    
    private TMP_InputField[] _inputFields;
    private int _inputFieldIndex;

    private UiManager _uiManagerInstance;
    
    private InputActionReference MoveUiAction
    {
        get
        {
            if (!_uiManagerInstance)
                _uiManagerInstance = FindFirstObjectByType<UiManager>();
            
            AccessTools.FieldRef<UiManager, InputActionReference> moveUiAction =
                AccessTools.FieldRefAccess<UiManager, InputActionReference>("moveUiAction");
            
            return moveUiAction(_uiManagerInstance);
        }
    }
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        SceneManager.sceneLoaded += (scene, _) =>
        {
            if (scene.name == "MainMenu")
                CreateConnectionGUI();
        };
        
        var harmony = new Harmony("PunkArchipelago");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
    
    private void Update()
    {
        TabToNextField();
    }
    
    private void TabToNextField()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;
        if (_inputFieldIndex == -1)
            return;

        int delta = Input.GetKey(KeyCode.LeftShift) ? -1 : 1;
        if (_inputFieldIndex + delta < 0 || _inputFieldIndex + delta >= _inputFields.Length)
            return;
        
        var targetInputField = _inputFields[_inputFieldIndex += delta];
        EventSystem.current.SetSelectedGameObject(targetInputField.gameObject);
    }

    public void CreateConnectionGUI()
    {
        var assetBundle = AssetBundleHelper.GetAssetBundle("connectionguibundle");
        GameObject prefab = assetBundle.LoadAsset<GameObject>("ArchipelagoGUI");

        var guiInstance = Instantiate(prefab);
        var interactables = guiInstance.transform.Find("Interactables");
        
        _inputFields = interactables.GetComponentsInChildren<TMP_InputField>();
        for (int i = 0; i < _inputFields.Length; i++)
        {
            var inputField = _inputFields[i];

            var iSnapshot = i;
            inputField.onSelect.AddListener(delegate
            {
                MoveUiAction.action.Disable();
                _inputFieldIndex = iSnapshot;
            });
            inputField.onDeselect.AddListener(delegate
            {
                MoveUiAction.action.Enable();
                _inputFieldIndex = -1;
            });
        }

        _inputFields[0].onValueChanged.AddListener(text => ArchipelagoConnection.HOST = text);
        _inputFields[1].onValueChanged.AddListener(text => ArchipelagoConnection.PORT = text);
        _inputFields[2].onValueChanged.AddListener(text => ArchipelagoConnection.SLOT = text);
        _inputFields[3].onValueChanged.AddListener(text => ArchipelagoConnection.PASS = text);

        var deathLinkToggle = interactables.Find("Toggle").GetComponent<Toggle>();
        deathLinkToggle.onValueChanged.AddListener(on => ArchipelagoConnection.DEATH_LINK = on);

        var button = interactables.Find("Connect Button").GetComponent<Button>();
        button.onClick.AddListener(async () => await ArchipelagoConnection.TryConnect());
    }
}
