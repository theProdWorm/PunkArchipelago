using HarmonyLib;
using PunkArchipelago.Archipelago;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(PauseScreen), nameof(PauseScreen.Close))]
public class PauseScreen_Close
{
    [HarmonyPostfix]
    private static void Close()
    {
        ItemHandler.Update();
    }
}