using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Packets;
using MonoMod.Utils;

namespace PunkArchipelago.Archipelago;

public static class LocationHandler
{
    private static Dictionary<string, Queue<long>> _REMAINING_INCREMENTAL_LOCATIONS;
    private static Dictionary<string, long> _REMAINING_SHOP_LOCATIONS;

    private static Dictionary<string, long> _LOCATION_NAME_TO_ID;
    
    public static void Initialize(ArchipelagoSession session)
    {
        _LOCATION_NAME_TO_ID = session.Locations.AllLocations.ToDictionary(
            locationId => session.Locations.GetLocationNameFromId(locationId, "PUNK"),
            locationId => locationId);

        var remainingLocations = session.Locations.AllMissingLocations.ToHashSet();
        
        _REMAINING_INCREMENTAL_LOCATIONS = new();
        
        var incrementalLocations = _LOCATION_NAME_TO_ID
            .Where(location => IsIncrementalLocation(location.Key))
            .Where(location => remainingLocations.Contains(location.Value))
            .OrderBy(location => location.Value);
        foreach (var location in incrementalLocations)
        {
            string locationName = StripTrailingNumber(location.Key);
            
            _REMAINING_INCREMENTAL_LOCATIONS.TryAdd(locationName, new Queue<long>());
            _REMAINING_INCREMENTAL_LOCATIONS[locationName].Enqueue(location.Value);
        }

        _REMAINING_SHOP_LOCATIONS = _LOCATION_NAME_TO_ID
            .Where(location => IsShopLocation(location.Key))
            .Where(location => remainingLocations.Contains(location.Value))
            .ToDictionary(
                location => location.Key,
                location => location.Value);

        foreach (var location in _REMAINING_SHOP_LOCATIONS)
        {
            Plugin.LOGGER.LogInfo(location.Key);
        }
        
        Plugin.LOGGER.LogInfo($"Total Locations: {_LOCATION_NAME_TO_ID.Count}");
        int incrementalLocationCount = _REMAINING_INCREMENTAL_LOCATIONS.Aggregate(0, 
            (total, current) => current.Value.Count + total);
        Plugin.LOGGER.LogInfo($"Total Incremental Locations: {incrementalLocationCount}");
        Plugin.LOGGER.LogInfo($"Total Shop Locations: {_REMAINING_SHOP_LOCATIONS.Count}");
    }

    public static bool TryCheckLocation(string locationType)
    {
        if (!TryGetNextId(locationType, out var locationId))
            return false;
        
        CheckLocation(locationId);
        return true;
    }
    
    private static void CheckLocation(long locationId)
    {
        PacketHandler.CheckLocations(locationId);
    }
    
    public static string GetLocationType(string unitName)
    {
        if (unitName.Contains("Crate Level 2"))
            return "Generator Crate";
        if (unitName.Contains("Crate"))
        {
            if (Options.SEPARATE_COLORED_CRATES)
            {
                if (unitName.Contains("Green"))
                    return "Green Crate";
                if (unitName.Contains("Caps"))
                    return "Orange Crate";
                if (unitName.Contains("Purple"))
                    return "Pink Crate";
                if (unitName.Contains("Tech"))
                    return "Purple Crate";
                if (unitName.Contains("Gold")) //Unsure
                    return "Yellow Crate";
                
                return "White Crate";
            }
            
            return "Crate";
        }
        
        if (unitName.Contains("NippleCurved"))
            return "Fiber Plant";
        if (unitName.Contains("Sage"))
            return "Gland Plant";
        
        if (unitName.Contains("Small"))
            return "Small Encounter";
        if (unitName.Contains("Red"))
            return "Advanced Encounter";
        if (unitName.Contains("Queen"))
            return "Final Boss";

        return null;
    }

    private static string StripTrailingNumber(string str)
    {
        string stripped = string.Copy(str);
        for (int i = stripped.Length - 1; i >= 0; i--)
        {
            if (int.TryParse(stripped[i].ToString(), out _))
                stripped = stripped.Remove(i);
            else break;
        }
        return stripped.Trim();
    }

    private static bool IsIncrementalLocation(string locationName) =>
        !IsShopLocation(locationName) && (
            locationName.Contains("Crate") ||
            locationName.Contains("Plant") ||
            locationName.Contains("Box") ||
            locationName.Contains("Encounter") ||
            locationName.Contains("Boss"));
    
    private static bool IsShopLocation(string locationName) => 
        locationName.Contains("Post");

    private static bool TryGetNextId(string locationType, out long id)
    {
        if (!_REMAINING_INCREMENTAL_LOCATIONS.TryGetValue(locationType, out var ids) || !ids.Any())
        {
            id = long.MaxValue;
            return false;
        }
        
        id = GetNextId(locationType);
        return true;
    }

    private static long GetNextId(string locationType)
        => _REMAINING_INCREMENTAL_LOCATIONS[locationType].Dequeue();
}