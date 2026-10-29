using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace PunkArchipelago.Enemies;

[HarmonyPatch(typeof(DamagableResource), "Update")]
public class DamagableResource_Update
{
    private static readonly HashSet<DamagableResource> _PRINTED = [];
    
    private static void Postfix(DamagableResource __instance)
    {
        // Plugin.LOGGER.LogInfo($"Health.Awake called on {__instance.name}");

        if (!_PRINTED.Contains(__instance))
        {
            Plugin.LOGGER.LogInfo(__instance.name + " is a DamagableResource.");
            _PRINTED.Add(__instance);
        }
        
        // if (__instance.name.Contains("Fruit"))
        //     return;
        //
        // var componentNames = string.Join(",", __instance.gameObject.GetComponents<Object>()
        //     .Select(obj => obj.GetType().Name));
        //
        // Plugin.LOGGER.LogInfo($"{__instance.name} has components: {componentNames}");
    }
}