using HarmonyLib;
using PunkArchipelago.Archipelago;

namespace PunkArchipelago.Loot;

[HarmonyPatch(typeof(LootDropper), "DropLoot")]
public class LootDropper_DropLoot
{
    private static void Prefix(LootDropper __instance)
    {
        Plugin.LOGGER.LogInfo($"DropLoot called on {__instance.name}");

        if (!ConnectionHandler.IS_CONNECTED)
        {
            Plugin.LOGGER.LogInfo("Not connected to Archipelago server");
            goto skip;
        }
        
        var locationType = LocationHandler.GetLocationType(__instance.name);
        if (locationType == LocationHandler.LocationType.Invalid)
            goto skip;

        int remainingLocations = LocationHandler.GetRemainingLocations(locationType);
        if (remainingLocations <= 0)
            goto skip;

        LocationHandler.CheckIncrementalLocation(locationType);
        return;
        
        skip:
        Utils.DoNothing();
        
        Plugin.LOGGER.LogInfo("Skipping DropLoot");
    }
}