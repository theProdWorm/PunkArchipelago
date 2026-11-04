using HarmonyLib;
using PunkArchipelago.Archipelago;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(LoadoutSelector), "PickLoadout")]
public class LoadoutSelector_PickLoadout
{
    [HarmonyPostfix]
    private static void PickLoadout()
    {
        Plugin.LOGGER.LogWarning("New run started");
        ConnectionHandler.OnNewRun();
    }
}