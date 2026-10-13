using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;

namespace PunkArchipelago;

[BepInProcess("Punk.exe")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ArchipelagoPlugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        var harmony = new Harmony("PunkArchipelago");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
