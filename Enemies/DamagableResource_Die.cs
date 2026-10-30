using HarmonyLib;

namespace PunkArchipelago.Enemies;

[HarmonyPatch(typeof(DamagableResource), nameof(DamagableResource.Die))]
public class DamagableResource_Die
{
    [HarmonyPostfix]
    private static void Die(DamagableResource __instance)
    {
        // if (__instance.)
    }
}