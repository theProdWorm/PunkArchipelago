using System.Collections.Generic;

namespace PunkArchipelago.Archipelago;

public static class LocationHandler
{
    private static Dictionary<LocationType, int> _REMAINING_LOCATIONS;
    
    public static void Initialize()
    {
        _REMAINING_LOCATIONS = new()
        {
            { LocationType.GeneratorCache, Options.GENERATOR_CACHES },
            { LocationType.FiberPlant, Options.FIBER_PLANTS },
            { LocationType.GlandPlant, Options.GLAND_PLANTS },
            { LocationType.SmallEncounter, Options.SMALL_ENCOUNTERS },
            { LocationType.AdvancedEncounter, Options.ADVANCED_ENCOUNTERS },
            { LocationType.FinalBoss, Options.FINAL_BOSSES }
        };
    }

    public static void CheckLocation(LocationType locationType)
    {
        
    }
    
    public static LocationType GetLocationType(string unitName)
    {
        if (unitName.Contains("Crate Level 2"))
            return LocationType.GeneratorCache;
        if (unitName.Contains("Crate"))
        {
            if (Options.SEPARATE_COLORED_CACHES)
            {
                if (unitName.Contains("White")) //Unsure
                    return LocationType.WhiteCrate;
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
            }
            else
                return LocationType.Crate;
        }
        
        if (unitName.Contains("NippleCurved"))
            return LocationType.FiberPlant;
        if (unitName.Contains("___Gland"))
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
        if (_REMAINING_LOCATIONS.TryGetValue(locationType, out var remainingLocations)) 
            return remainingLocations;
        
        Plugin.LOGGER.LogError($"Invalid location type {locationType}");
        return -1;
    }

    public enum LocationType
    {
        GeneratorCache,
        Crate,
        WhiteCrate,
        GreenCrate,
        OrangeCrate,
        PinkCrate,
        PurpleCrate,
        YellowCrate,
        FiberPlant,
        GlandPlant,
        SmallEncounter,
        AdvancedEncounter,
        FinalBoss,
        Shop,
        Invalid
    }

}