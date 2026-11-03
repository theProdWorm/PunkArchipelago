using HarmonyLib;

namespace PunkArchipelago.Vault;

[HarmonyPatch(typeof(global::Vault), nameof(global::Vault.RestoreFromMemento))]
public class Vault_RestoreFromMemento
{
    [HarmonyPrefix]
    private static void RestoreFromMemento(global::Vault.Memento memento)
    {
        // IRegistry<ModuleData, string> moduleRegistry = ServiceLocator.Get<IRegistry<ModuleData, string>>();
        // IRegistry<Ingredient, string> ingredientRegistry = ServiceLocator.Get<IRegistry<Ingredient, string>>();
        // IRegistry<Consumable, string> consumableRegistry = ServiceLocator.Get<IRegistry<Consumable, string>>();
        // foreach (var moduleMemento in memento.modules)
        // {
        //     var module = moduleRegistry.Get(moduleMemento.moduleDataId);
        //     Plugin.LOGGER.LogInfo($"name: {module.displayName}, id: {module.Id}");
        // }
        // foreach (var ingredientId in memento.ingredientIds)
        // {
        //     var module = moduleRegistry.Get(moduleMemento.moduleDataId);
        //     Plugin.LOGGER.LogInfo($"name: {module.displayName}, id: {module.Id}");
        // }
        //
        //
    }
}