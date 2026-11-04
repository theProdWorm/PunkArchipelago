using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(IngredientsBar), "AddItemsFromVault")]
public class IngredientsBar_AddItemsFromVault
{
    [HarmonyPrefix]
    private static bool AddItemsFromVault(IngredientsBar __instance)
    {
        var items = AccessTools.FieldRefAccess<Dictionary<Ingredient, IngredientBarItem>>
                (typeof(IngredientsBar), "items")(__instance);
        var itemPrefab = AccessTools.FieldRefAccess<IngredientBarItem>(typeof(IngredientsBar), "itemPrefab")(__instance);
        
        var vault = ServiceLocator.Get<Vault>();
        
        foreach (KeyValuePair<Ingredient, int> keyValuePair in vault.Ingredients)
        {
            if (items.ContainsKey(keyValuePair.Key))
                continue;
            
            IngredientBarItem ingredientBarItem = Object.Instantiate(itemPrefab, __instance.transform);
            ingredientBarItem.SetIngredient(keyValuePair.Key);
            ingredientBarItem.SetAmount(keyValuePair.Value);
            items.Add(keyValuePair.Key, ingredientBarItem);
        }

        return false;
    }
}