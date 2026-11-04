using System.Collections.Generic;
using System.IO;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace PunkArchipelago.Archipelago;

public static class ItemHandler
{
    private static Dictionary<long, int> _COLLECTED_ITEMS = new();

    private static readonly Dictionary<ItemType, string[]> _MODULE_ID_TABLE = new()
    {
        { ItemType.BasicShipModule, [
            "ed74decf-4507-46a1-a41b-9b17ca155d8e",
            "9ebbe694-85cd-46de-ba7a-2bfd5934e50c",
            "4618af6d-c054-4157-a4d0-ae0cdf093f33"
        ]},
        { ItemType.BasicWeaponModule, [
            "351d0570-5119-4dc9-8c4a-04ebe9d65eeb",
            "351d0570-5119-4dc9-8c4a-04ebe9d65eeb",
            "351d0570-5119-4dc9-8c4a-04ebe9d65eeb",
            "699decaa-30d6-4b73-970c-d0bc606f83a9",
            "699decaa-30d6-4b73-970c-d0bc606f83a9",
            "699decaa-30d6-4b73-970c-d0bc606f83a9",
            "2e9bd982-221c-48a2-8cad-deecd40bbd7f",
            "2e9bd982-221c-48a2-8cad-deecd40bbd7f",
            "df92003e-2ffa-467a-b8bc-0936515311ea",
            "df92003e-2ffa-467a-b8bc-0936515311ea",
            "df92003e-2ffa-467a-b8bc-0936515311ea"
        ]},
        { ItemType.GoodWeaponModule, [
            "bcc06d5e-5a27-4313-ae42-dd07877df2bb"
        ]},
        { ItemType.ResourceModule, [
            "97b5d6c6-fdee-4e07-9f1c-e7eb71018ab8",
            "446fc56b-ec1f-480f-ac9f-d356458f7a45",
            "d4e6117e-75d9-490e-bbc6-f6a683b2fd48",
            "4193938c-ea4f-4fb3-8f06-1147eb33bf42"
        ]},
        { ItemType.OnHitModule, [
            "28228fa7-5770-4380-b454-f75cd2bbca53",
            "1e76dca3-2972-4d83-95ec-81519216c698",
            "8598ed65-8b4b-4f59-8aef-58cbb137fc8d",
            "199b8db0-b3db-4d84-91ad-ab46a32341c4",
            "869b8723-9f5b-4cb2-9e3d-bd9881f29b20",
            "123b8db0-b3db-4d14-91ad-ab46a34341c4"
        ]},
        { ItemType.BasicRegenModule, [
            "fa46fc36-1e37-4c91-a319-172285b9c3dd",
            "fa46fc36-1e37-4c91-a319-172285b9c3dd",
            "1a00aa71-b1e5-4a5c-806c-5d8d30a1353a",
            "1a00aa71-b1e5-4a5c-806c-5d8d30a1353a",
            "dd7c4547-8332-4167-b67d-7b5445b5d8a3"
        ]},
        { ItemType.GoodRegenModule, [
            "c0c721ff-3be0-4e4e-a2e1-dff7b1a11230",
            "8351e3d0-20bf-4f6d-a97a-2322dd76b596"
        ]},
        { ItemType.ShieldModule, [
            "48a433fa-b05d-42e7-b273-b95046c4cef8",
            "48a433fa-b05d-42e7-b273-b95046c4cef8",
            "bf8fb7e6-d7fe-4e10-a261-abf1843909a9",
            "c792bc27-6cc0-4162-bf25-55720fdcc9d4",
            "c792bc27-6cc0-4162-bf25-55720fdcc9d4",
            "04e1c99f-6b61-4008-9b0b-e4ab7c6aa4c0"
        ]},
        { ItemType.Core, [
            "c8da87d5-997b-4d1c-aefb-5242051a10d7",
            "c8da87d5-997b-4d1c-aefb-5242051a10d7",
            "e070f3d8-c876-440d-a66e-7e9d8fd8b6ca"
        ]}
    };

    private static readonly Dictionary<ItemType, string[]> _INGREDIENT_ID_TABLE = new()
    {
        { ItemType.Powerstar, ["Powerstar"]},
        { ItemType.Fiber, ["Coral"]},
        { ItemType.Gland, ["Gland"]},
        { ItemType.Chip, ["Chip"]},
        { ItemType.Generator, ["Shell"]}
    };

    private static readonly Dictionary<ItemType, string[]> _RESOURCE_ID_TABLE = new()
    {
        { ItemType.Health, ["Resource Health"] },
        { ItemType.Fuel, ["Resource Fuel"] },
        { ItemType.Gold, ["Resource Money"] },
    };

    private static readonly Dictionary<ItemType, string[]> _CONSUMABLE_ID_TABLE = new()
    {
        { ItemType.Consumable, [
            "3cb2c73b-3037-4f99-aa25-d60d24a78f75",
            "6681e231-1ef9-4e82-96c7-e6c1611e4b19",
            "6fbca6a7-6178-4c6e-8ff7-448b0d6c7658",
            "ad411315-6fe7-4d87-90e8-2ca0c418cf1b"
        ]}
    };

    private static readonly List<ModuleData> _NEW_MODULES     = new();
    private static readonly List<Ingredient> _NEW_INGREDIENTS = new();
    private static readonly List<Resource>   _NEW_RESOURCES   = new();
    private static readonly List<Consumable> _NEW_CONSUMABLES = new();

    private static int _COMPLETED_FINAL_BOSSES;
    
    private static string _PATH;

    private static readonly List<long> _ADDED_ITEMS = new();

    /// <summary>
    /// Call when a new connection is established on a different multiworld
    /// </summary>
    internal static void Initialize(ItemInfo[] collectedItems, string seed, bool ignoreSave = false)
    {
        string dir = "BepInEx/plugins/PUNK Archipelago/data";
        _PATH = Path.Combine(dir, $"{seed}.json");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        
        _COLLECTED_ITEMS.Clear();
        _NEW_MODULES.Clear();
        _NEW_INGREDIENTS.Clear();
        _NEW_RESOURCES.Clear();
        _NEW_CONSUMABLES.Clear();
        _ADDED_ITEMS.Clear();

        var collectedItemsArray = collectedItems.ToArray();
        Plugin.LOGGER.LogInfo($"All items ({collectedItemsArray.Length}):");
        foreach (var item in collectedItemsArray.OrderBy(x => x.ItemId))
        {
            Plugin.LOGGER.LogInfo(item.ItemName);
        }
        
        if (!ignoreSave && File.Exists(_PATH))
        {
            if (collectedItemsArray.Length == 0)
            {
                File.Delete(_PATH);
            }
            else
            {
                Plugin.LOGGER.LogInfo($"reading json from {seed}.json");
                
                string json = File.ReadAllText(_PATH);
                _COLLECTED_ITEMS = JsonConvert.DeserializeObject<Dictionary<long, int>>(json);
            }
        }
        
        Reinitialize(collectedItemsArray);
        
        Directory.GetFiles(dir, "*.json")
            .Where(file => file != _PATH)
            .ToList().ForEach(File.Delete);
    }

    /// <summary>
    /// Call when a new connection is established on the same multiworld
    /// </summary>
    internal static void Reinitialize(ItemInfo[] collectedItems)
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
                HandleItem(item.Key);
            }
        }
    }

    internal static void SerializeCollectedItems()
    {
        if (_PATH == null)
            return;
        
        string json = JsonConvert.SerializeObject(_COLLECTED_ITEMS);
        if (!File.Exists(_PATH))
            File.Create(_PATH).Close();
        
        File.WriteAllText(_PATH, json);
    }

    internal static void Update()
    {
        if (!ConnectionHandler.IS_CONNECTED)
            return;
        
        if (!ServiceLocator.TryGet(out Vault vault) || 
            !ServiceLocator.TryGet(out ShipManager shipManager))
            return;

        for (int i = _NEW_MODULES.Count - 1; i >= 0; i--)
        {
            var moduleData = _NEW_MODULES[i];
            var module = new Module(moduleData);
            module.RandomizeConnections();
            
            vault.Store(module, true);
            _NEW_MODULES.RemoveAt(i);
        }

        for (int i = _NEW_INGREDIENTS.Count - 1; i >= 0; i--)
        {
            var ingredient = _NEW_INGREDIENTS[i];
            
            vault.Add(ingredient, 1);
            _NEW_INGREDIENTS.RemoveAt(i);
        }
        
        for (int i = _NEW_RESOURCES.Count - 1; i >= 0; i--)
        {
            var resource = _NEW_RESOURCES[i];
            
            foreach (var ship in shipManager.Ships)
            {
                ship.Unit.GetTank(resource).Charge(1000);
                if (resource.isShared)
                    break;
            }

            _NEW_RESOURCES.RemoveAt(i);
        }

        for (int i = _NEW_CONSUMABLES.Count - 1; i >= 0; i--)
        {
            var consumable = _NEW_CONSUMABLES[i];
            
            // TODO: fix this shit
            
            vault.Add(consumable, 1);
            _NEW_CONSUMABLES.RemoveAt(i);
        }

        if (_COMPLETED_FINAL_BOSSES >= Options.FINAL_BOSSES)
            PacketHandler.CompleteGoal();

        // if (_ADDED_ITEMS.Count == 0)
        //     return;
        //
        // Plugin.LOGGER.LogInfo($"Collected items vvv ({_ADDED_ITEMS.Count})");
        // foreach (var t in _ADDED_ITEMS.OrderBy(x => x))
        // {
        //     Plugin.LOGGER.LogInfo($"{t}: {(ItemType)t}");
        // }
        // _ADDED_ITEMS.Clear();
        // Plugin.LOGGER.LogInfo("Collected items ^^^");
    }
    
    public static void HandleItem(long archipelagoId, bool collect = true)
    {
        if (collect && !_COLLECTED_ITEMS.TryAdd(archipelagoId, 1))
            _COLLECTED_ITEMS[archipelagoId]++;
        
        var group = GetGroup(archipelagoId); 
        var itemType = (ItemType) archipelagoId;
        switch (group)
        {
            case ItemGroup.Module:
                CreateModule(itemType);
                break;
            case ItemGroup.Ingredient:
                CreateIngredient(itemType);
                break;
            case ItemGroup.Filler:
                CreateResource(itemType);
                break;
            case ItemGroup.Consumable:
                CreateConsumable(itemType);
                break;
            case ItemGroup.Boss:
                HandleBossItem(itemType);
                break;
            default:
                Plugin.LOGGER.LogError("Invalid ItemGroup: " + group);
                break;
        }
        
        _ADDED_ITEMS.Add(archipelagoId);
    }

    private static void CreateModule(ItemType itemType)
    {
        var ids = _MODULE_ID_TABLE[itemType];
        string id = ids[Random.Range(0, ids.Length)];

        var moduleData = ServiceLocator.Get<ModuleRegistry>().Get(id);
        _NEW_MODULES.Add(moduleData);
    }

    private static void CreateIngredient(ItemType itemType)
    {
        var ids = _INGREDIENT_ID_TABLE[itemType];
        string id = ids[Random.Range(0, ids.Length)];
        
        var ingredient = ServiceLocator.Get<IngredientRegistry>().Get(id);
        _NEW_INGREDIENTS.Add(ingredient);
    }

    private static void CreateResource(ItemType itemType)
    {
        var ids = _RESOURCE_ID_TABLE[itemType];
        string id = ids[Random.Range(0, ids.Length)];
        
        var resource = ServiceLocator.Get<ResourceRegistry>().Get(id);
        _NEW_RESOURCES.Add(resource);
    }

    private static void CreateConsumable(ItemType itemType)
    {
        var ids = _CONSUMABLE_ID_TABLE[itemType];
        string id = ids[Random.Range(0, ids.Length)];
            
        var consumable = ServiceLocator.Get<ConsumableRegistry>().Get(id);
        _NEW_CONSUMABLES.Add(consumable);
    }
    
    private static void HandleBossItem(ItemType itemType)
    {
        // if replace boss drops with shop items...

        if (itemType == ItemType.CompletedFinalBoss)
            _COMPLETED_FINAL_BOSSES++;
    }
    
    public static ItemGroup GetGroup(long itemId)
        => itemId switch
        {
            > 500 => ItemGroup.Boss,
            > 400 => ItemGroup.Consumable,
            > 300 => ItemGroup.Filler,
            > 200 => ItemGroup.Ingredient,
            > 100 => ItemGroup.Module
        };
    
    private enum ItemType
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
        
        Consumable = 401,
    
        CompletedSmallEncounter = 501,
        CompletedAdvancedEncounter = 502,
        CompletedFinalBoss = 503
    }

    public enum ItemGroup
    {
        Module,
        Ingredient,
        Filler,
        Consumable,
        Boss
    }
}