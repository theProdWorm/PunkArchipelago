using System.Collections.Generic;
using HarmonyLib;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(ConsumableWheel), "OnConsumableAmountChanged")]
public class ConsumableWheel_OnConsumableAmountChanged
{
    [HarmonyPrefix]
    private static bool OnConsumableAmountChanged(ConsumableWheel __instance)
    {
        var items = AccessTools.FieldRefAccess<List<ConsumableWheelItem>>(typeof(ConsumableWheel), "items")(__instance);
        return items.Count != 0;
    }
}