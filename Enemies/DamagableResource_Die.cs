using HarmonyLib;

namespace PunkArchipelago.Enemies;

[HarmonyPatch(typeof(DamagableResource), nameof(DamagableResource.Die))]
public class DamagableResource_Die
{
    private static void Postfix(DamagableResource __instance)
    {
        // if (__instance.)
    }
}