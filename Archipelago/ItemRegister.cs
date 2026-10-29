using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;

namespace PunkArchipelago.Archipelago;

public static class ItemRegister
{
    private static readonly Dictionary<string, int> _itemLookup = new();

    internal static void Initialize(IEnumerable<ItemInfo> collectedItems)
    {
        _itemLookup.Clear();
        
        foreach (var item in collectedItems)
        {
            if (!_itemLookup.TryAdd(item.ItemName, 1))
                _itemLookup[item.ItemName]++;
        }
    }

    internal static void Update(IEnumerable<ItemInfo> collectedItems)
    {
        var deltaItems = _itemLookup.ToDictionary(item => item.Key, item => item.Value);
        var postItems = new Dictionary<string, int>();

        foreach (var item in collectedItems)
        {
            if (!postItems.TryAdd(item.ItemName, 1))
                postItems[item.ItemName]++;
        }

        var diff = postItems.ToDictionary(
            item => item.Key,
            item => item.Value + deltaItems.GetValueOrDefault(item.Key));
        
        // TODO: Check if game is currently paused.
        // If game is paused, add a listener to receive items on unpause
    }

    internal static void AddItemToVault(ItemInfo item)
    {
        
    }

    private static void ToPunkItem(this ItemInfo item)
    {
        switch ((ItemLUT) item.ItemId)
        {
            
        }
    }

    private enum ItemLUT
    {
        CommonShipModule = 101,
        CommonWeaponModule = 102,
        GoodWeaponModule = 103,
        ResourceModule = 104,
        OnHitModule = 105,
        BasicRegenModule = 106,
        StaminaRegenModule = 107,
    
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
}