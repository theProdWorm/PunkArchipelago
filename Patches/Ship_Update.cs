using HarmonyLib;
using PunkArchipelago.Archipelago;
using UnityEngine;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(Ship), "Update")]
public class Ship_Update
{
    [HarmonyPostfix]
    private static void Update(Ship __instance)
    {
        ItemHandler.Update();
    }
}