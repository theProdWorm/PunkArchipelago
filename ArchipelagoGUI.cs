using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PunkArchipelago;

[BepInProcess("Punk.exe")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArchipelagoGUI : BaseUnityPlugin
{
    private static List<InputAction> ENABLED_ACTIONS;
    
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
        SceneManager.sceneLoaded += (scene, _) =>
        {
            if (scene.name == "MainMenu")
                CreateConnectionGUI();
        };
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
    }
}