using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PunkArchipelago.Archipelago;

public static class ItemHandler
{
    private static readonly Dictionary<long, int> _COLLECTED_ITEMS = new();

    private static readonly Dictionary<ItemLUT, string[]> _MODULE_ID_TABLE = new()
    {
        { ItemLUT.BasicShipModule, [
            "ed74decf-4507-46a1-a41b-9b17ca155d8e",
            "9ebbe694-85cd-46de-ba7a-2bfd5934e50c",
            "4618af6d-c054-4157-a4d0-ae0cdf093f33"
        ]},
        { ItemLUT.BasicWeaponModule, [
            "351d0570-5119-4dc9-8c4a-04ebe9d65eeb",
            "699decaa-30d6-4b73-970c-d0bc606f83a9",
            "2e9bd982-221c-48a2-8cad-deecd40bbd7f",
            "df92003e-2ffa-467a-b8bc-0936515311ea"
        ]},
        { ItemLUT.GoodWeaponModule, [
            "bcc06d5e-5a27-4313-ae42-dd07877df2bb"
        ]},
        { ItemLUT.ResourceModule, [
            "97b5d6c6-fdee-4e07-9f1c-e7eb71018ab8",
            "446fc56b-ec1f-480f-ac9f-d356458f7a45",
            "d4e6117e-75d9-490e-bbc6-f6a683b2fd48",
            "4193938c-ea4f-4fb3-8f06-1147eb33bf42"
        ]},
        { ItemLUT.OnHitModule, [
            "28228fa7-5770-4380-b454-f75cd2bbca53",
            "1e76dca3-2972-4d83-95ec-81519216c698",
            "8598ed65-8b4b-4f59-8aef-58cbb137fc8d",
            "199b8db0-b3db-4d84-91ad-ab46a32341c4",
            "869b8723-9f5b-4cb2-9e3d-bd9881f29b20",
            "123b8db0-b3db-4d14-91ad-ab46a34341c4"
        ]},
        { ItemLUT.BasicRegenModule, [
            "fa46fc36-1e37-4c91-a319-172285b9c3dd",
            "1a00aa71-b1e5-4a5c-806c-5d8d30a1353a",
            "dd7c4547-8332-4167-b67d-7b5445b5d8a3"
        ]},
        { ItemLUT.GoodRegenModule, [
            "c0c721ff-3be0-4e4e-a2e1-dff7b1a11230",
            "8351e3d0-20bf-4f6d-a97a-2322dd76b596"
        ]},
        { ItemLUT.ShieldModule, [
            "48a433fa-b05d-42e7-b273-b95046c4cef8",
            "bf8fb7e6-d7fe-4e10-a261-abf1843909a9",
            "c792bc27-6cc0-4162-bf25-55720fdcc9d4",
            "04e1c99f-6b61-4008-9b0b-e4ab7c6aa4c0"
        ]},
        { ItemLUT.Core, [
            "c8da87d5-997b-4d1c-aefb-5242051a10d7",
            "e070f3d8-c876-440d-a66e-7e9d8fd8b6ca"
        ]}
    };

    private static readonly Dictionary<ItemLUT, string[]> _INGREDIENT_ID_TABLE = new()
    {
        { ItemLUT.Powerstar, ["Powerstar"]},
        { ItemLUT.Fiber, ["Coral"]},
        { ItemLUT.Gland, ["Gland"]},
        { ItemLUT.Chip, ["Chip"]},
        { ItemLUT.Generator, ["Shell"]}
    };

    private static readonly Dictionary<ItemLUT, string[]> _FILLER_ID_TABLE = new()
    {
        { ItemLUT.Health, ["Resource Health"] },
        { ItemLUT.Fuel, ["Resource Fuel"] },
        { ItemLUT.Gold, ["Resource Money"] },
        { ItemLUT.Gel, ["Resource Purple"] },
        { ItemLUT.Caps, ["Resource Caps"] },
        { ItemLUT.Tech, ["Resource Tech"] },
        { ItemLUT.Consumable, [
            "3cb2c73b-3037-4f99-aa25-d60d24a78f75",
            "6681e231-1ef9-4e82-96c7-e6c1611e4b19",
            "6fbca6a7-6178-4c6e-8ff7-448b0d6c7658",
            "ad411315-6fe7-4d87-90e8-2ca0c418cf1b"
        ] }
    };

    private static readonly List<ModuleData> _NEW_MODULES = new();
    
    /// <summary>
    /// Call when a new connection is established on a different multiworld
    /// </summary>
    /// <param name="collectedItems">All items collected</param>
    internal static void Initialize(IEnumerable<ItemInfo> collectedItems)
    {
        _COLLECTED_ITEMS.Clear();
        
        foreach (var item in collectedItems)
        {
            if (!_COLLECTED_ITEMS.TryAdd(item.ItemId, 1))
                _COLLECTED_ITEMS[item.ItemId]++;
            
            CreateItem(item.ItemId, GetGroup(item.ItemId));
        }
    }

    /// <summary>
    /// Call when a new connection is established on the same multiworld
    /// </summary>
    /// <param name="collectedItems">All items collected</param>
    internal static void Reinitialize(IEnumerable<ItemInfo> collectedItems)
    {
        var deltaItems = _COLLECTED_ITEMS;
        var postItems = new Dictionary<long, int>();

        foreach (var item in collectedItems)
        {
            if (!postItems.TryAdd(item.ItemId, 1))
                postItems[item.ItemId]++;
        }

        var diff = postItems.ToDictionary(
            item => item.Key,
            item => item.Value - deltaItems.GetValueOrDefault(item.Key));

        foreach (var item in diff)
        {
            for (int i = 0; i < item.Value; i++)
            {
                CreateItem(item.Key, GetGroup(item.Key));
            }
        }
        
        // TODO: Check if game is currently paused.
        // If game is paused, add a listener to receive items on unpause
    }

    internal static void Update()
    {
        for (int i = _NEW_MODULES.Count - 1; i >= 0; i--)
        {
            var moduleData = _NEW_MODULES[i];
            var module = new Module(moduleData);
            var vault = ServiceLocator.Get<global::Vault>();
            
            vault.Store(module, true);
            _NEW_MODULES.RemoveAt(i);
        }
    }
    
    public static void CreateItem(long archipelagoId, ItemGroup group)
    {
        var itemType = (ItemLUT) archipelagoId;
        switch (group)
        {
            case ItemGroup.Module:
                CreateModule(itemType);
                break;
            case ItemGroup.Ingredient:
                CreateIngredient(itemType);
                break;
            case ItemGroup.Filler:
                CreateFiller(itemType);
                break;
            default:
                Plugin.LOGGER.LogError("Unknown ItemGroup: " + group);
                break;
        }
    }

    private static void CreateModule(ItemLUT itemType)
    {
        var ids = _MODULE_ID_TABLE[itemType];
        string id = ids[Random.Range(0, ids.Length)];

        var moduleData = ServiceLocator.Get<ModuleRegistry>().Get(id);
        _NEW_MODULES.Add(moduleData);
    }

    private static void CreateIngredient(ItemLUT itemType)
    {
        
    }

    private static void CreateFiller(ItemLUT itemType)
    {
        
    }
    
    public static ItemGroup GetGroup(long itemId)
        => itemId switch
        {
            > 400 => ItemGroup.Boss,
            > 300 => ItemGroup.Filler,
            > 200 => ItemGroup.Ingredient,
            > 100 => ItemGroup.Module
        };
    
    private enum ItemLUT
    {
        BasicShipModule = 101,
        BasicWeaponModule = 102,
        GoodWeaponModule = 103,
        ResourceModule = 104,
        OnHitModule = 105,
        BasicRegenModule = 106,
        GoodRegenModule = 107,
        ShieldModule = 108,
        Core = 109,
    
        Powerstar = 201,
        Fiber = 202,
        Gland = 203,
        Chip = 204,
        Generator = 205,
    
        Health = 301,
        Fuel = 302,
        Gold = 303,
        Gel = 304,
        Caps = 305,
        Tech = 306,
        Consumable = 307,
    
        CompletedSmallEncounter = 401,
        CompletedAdvancedEncounter = 402,
        CompletedFinalBoss = 403
    }

    public enum ItemGroup
    {
        Module,
        Ingredient,
        Filler,
        Boss
    }
}