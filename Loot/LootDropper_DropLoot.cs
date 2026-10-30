using HarmonyLib;
using PunkArchipelago.Archipelago;

namespace PunkArchipelago.Loot;

[HarmonyPatch(typeof(LootDropper), "DropLoot")]
public class LootDropper_DropLoot
{
    [HarmonyPrefix]
    private static bool DropLoot(LootDropper __instance)
    {
        Plugin.LOGGER.LogInfo($"DropLoot called on {__instance.name}");

        if (!ConnectionHandler.IS_CONNECTED)
        {
            Plugin.LOGGER.LogInfo("Not connected to Archipelago server");
            return true;
        }
        
        string locationType = LocationHandler.GetLocationType(__instance.name);
        if (string.IsNullOrEmpty(locationType))
            return true;

        return !LocationHandler.TryCheckLocation(locationType);
    }
}