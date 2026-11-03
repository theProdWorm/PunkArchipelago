using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace PunkArchipelago.Vault;

[HarmonyPatch(typeof(global::Vault), nameof(global::Vault.Store))]
public class Vault_Store
{
    [HarmonyPrefix]
    private static void Store(global::Vault __instance, ref Module module)
    {
        Plugin.LOGGER.LogInfo($"Storing module: {module.DisplayName}, id: {module.Data.Id}");
        
    }
}