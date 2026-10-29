using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Packets;
using MonoMod.Utils;

namespace PunkArchipelago.Archipelago;

public static class LocationHandler
{
    private static Dictionary<LocationType, int> _CHECKED_INCREMENTAL_LOCATIONS;
    private static Dictionary<LocationType, int> _REMAINING_INCREMENTAL_LOCATIONS;
    
    public static void Initialize()
    {
        _REMAINING_INCREMENTAL_LOCATIONS = new()
        {
            { LocationType.GeneratorCrate, Options.GENERATOR_CRATES },
            { LocationType.FiberPlant, Options.FIBER_PLANTS },
            { LocationType.GlandPlant, Options.GLAND_PLANTS },
            { LocationType.SmallEncounter, Options.SMALL_ENCOUNTERS },
            { LocationType.AdvancedEncounter, Options.ADVANCED_ENCOUNTERS },
            { LocationType.FinalBoss, Options.FINAL_BOSSES }
        };

        if (Options.SEPARATE_COLORED_CRATES)
        {
            var crateLocations = new Dictionary<LocationType, int>()
            {
                { LocationType.WhiteCrate, Options.WHITE_CRATES},
                { LocationType.GreenCrate, Options.GREEN_CRATES },
                { LocationType.OrangeCrate, Options.ORANGE_CRATES },
                { LocationType.PinkCrate, Options.PINK_CRATES },
                { LocationType.PurpleCrate, Options.PURPLE_CRATES },
                { LocationType.YellowCrate, Options.YELLOW_CRATES },
            };
            _REMAINING_INCREMENTAL_LOCATIONS.AddRange(crateLocations);
        }
        else
        {
            _REMAINING_INCREMENTAL_LOCATIONS.Add(LocationType.Crate, Options.TOTAL_CRATES);
        }

        _CHECKED_INCREMENTAL_LOCATIONS = _REMAINING_INCREMENTAL_LOCATIONS.ToDictionary(location => location.Key, _ => 0);
    }

    public static void CheckIncrementalLocation(LocationType locationType)
    {
        long locationId = GetNextId(locationType);
        CheckLocation(locationId);
        
        _CHECKED_INCREMENTAL_LOCATIONS[locationType]++;
        _REMAINING_INCREMENTAL_LOCATIONS[locationType]--;
    }

    public static void CheckLocation(long locationId)
    {
        var packet = new LocationChecksPacket()
        {
            Locations = [locationId]
        };
        
        PacketHandler.SendPacket(packet);
    }
    
    public static LocationType GetLocationType(string unitName)
    {
        if (unitName.Contains("Crate Level 2"))
            return LocationType.GeneratorCrate;
        if (unitName.Contains("Crate"))
        {
            if (Options.SEPARATE_COLORED_CRATES)
            {
                if (unitName.Contains("Green"))
                    return LocationType.GreenCrate;
                if (unitName.Contains("Caps"))
                    return LocationType.OrangeCrate;
                if (unitName.Contains("Purple"))
                    return LocationType.PinkCrate;
                if (unitName.Contains("Tech"))
                    return LocationType.PurpleCrate;
                if (unitName.Contains("Gold")) //Unsure
                    return LocationType.YellowCrate;
                
                return LocationType.WhiteCrate;
            }
            
            return LocationType.Crate;
        }
        
        if (unitName.Contains("NippleCurved"))
            return LocationType.FiberPlant;
        if (unitName.Contains("Sage"))
            return LocationType.GlandPlant;
        
        if (unitName.Contains("Small"))
            return LocationType.SmallEncounter;
        if (unitName.Contains("Red"))
            return LocationType.AdvancedEncounter;
        if (unitName.Contains("Boss")) //Unsure
            return LocationType.FinalBoss;
        
        return LocationType.Invalid;
    }

    public static int GetRemainingLocations(LocationType locationType)
    {
        if (_REMAINING_INCREMENTAL_LOCATIONS.TryGetValue(locationType, out var remainingLocations)) 
            return remainingLocations;
        
        Plugin.LOGGER.LogError($"Invalid location type {locationType}");
        return -1;
    }
    
    private static long GetNextId(LocationType locationType)
    {
        long offset = GetIdOffset(locationType);
        int locationsChecked = _CHECKED_INCREMENTAL_LOCATIONS[locationType];
        
        return offset + locationsChecked;
    }
    
    private static long GetIdOffset(LocationType locationType)
        => locationType switch
        {
            LocationType.GeneratorCrate => 1,
            LocationType.Crate => 700,
            LocationType.WhiteCrate => 100,
            LocationType.GreenCrate => 200,
            LocationType.OrangeCrate => 300,
            LocationType.PinkCrate => 400,
            LocationType.PurpleCrate => 500,
            LocationType.YellowCrate => 600,
            LocationType.FiberPlant => 800,
            LocationType.GlandPlant => 900,
            LocationType.Box => 1000,
            LocationType.SmallEncounter => 1100,
            LocationType.AdvancedEncounter => 1200,
            LocationType.FinalBoss => 1300,
            _ => 0
        };
    
    public enum LocationType
    {
        GeneratorCrate,
        Crate,
        WhiteCrate,
        GreenCrate,
        OrangeCrate,
        PinkCrate,
        PurpleCrate,
        YellowCrate,
        FiberPlant,
        GlandPlant,
        Box,
        SmallEncounter,
        AdvancedEncounter,
        FinalBoss,
        Shop,
        Invalid
    }

}