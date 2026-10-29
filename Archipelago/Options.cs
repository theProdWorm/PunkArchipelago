using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PunkArchipelago.Archipelago;

public static class Options
{
    public enum DeathLinkPenalty
    {
        Damage = 0,
        One = 1,
        Death = 2
    }
    
    public static DeathLinkPenalty DEATH_LINK_PENALTY;
    public static int DEATH_LINK_DAMAGE;

    public static int SMALL_ENCOUNTERS;
    public static int ADVANCED_ENCOUNTERS;
    public static int FINAL_BOSSES;

    // public static bool ENCOUNTERS_UNLOCK_SHOP_ITEMS;
    // public static int SMALL_ENCOUNTER_SHOP_UNLOCKS;
    // public static int ADVANCED_ENCOUNTER_SHOP_UNLOCKS;
    // public static int FINAL_BOSS_SHOP_UNLOCKS;
    
    public static int GENERATOR_CRATES;

    public static bool SEPARATE_COLORED_CRATES;
    public static int WHITE_CRATES;
    public static int GREEN_CRATES;
    public static int ORANGE_CRATES;
    public static int PINK_CRATES;
    public static int PURPLE_CRATES;
    public static int YELLOW_CRATES;
    public static int TOTAL_CRATES;

    public static int FIBER_PLANTS;
    public static int GLAND_PLANTS;

    public static int BOXES;
    
    internal static void SetFrom(Dictionary<string, object> slotData)
    {
        DEATH_LINK_PENALTY = (DeathLinkPenalty) Convert.ToInt32(slotData["DeathLinkPenalty"]);
        DEATH_LINK_DAMAGE = Convert.ToInt32(slotData["DeathLinkDamage"]);
        
        SMALL_ENCOUNTERS = Convert.ToInt32(slotData["SmallEncounters"]);
        ADVANCED_ENCOUNTERS = Convert.ToInt32(slotData["AdvancedEncounters"]);
        FINAL_BOSSES = Convert.ToInt32(slotData["FinalBosses"]);
        
        // ENCOUNTERS_UNLOCK_SHOP_ITEMS = Convert.ToBoolean(slotData["EncountersUnlockShopItems"]);
        // SMALL_ENCOUNTER_SHOP_UNLOCKS = Convert.ToInt32(slotData["SmallEncounterShopUnlocks"]);
        // ADVANCED_ENCOUNTER_SHOP_UNLOCKS = Convert.ToInt32(slotData["AdvancedEncounterShopUnlocks"]);
        // FINAL_BOSS_SHOP_UNLOCKS = Convert.ToInt32(slotData["FinalBossShopUnlocks"]);
        
        GENERATOR_CRATES = Convert.ToInt32(slotData["GeneratorCrates"]);
        
        SEPARATE_COLORED_CRATES = Convert.ToBoolean(slotData["SeparateColoredCrates"]);
        if (SEPARATE_COLORED_CRATES)
        {
            WHITE_CRATES = Convert.ToInt32(slotData["WhiteCrates"]);
            GREEN_CRATES = Convert.ToInt32(slotData["GreenCrates"]);
            ORANGE_CRATES = Convert.ToInt32(slotData["OrangeCrates"]);
            PINK_CRATES = Convert.ToInt32(slotData["PinkCrates"]);
            PURPLE_CRATES = Convert.ToInt32(slotData["PurpleCrates"]);
            YELLOW_CRATES = Convert.ToInt32(slotData["YellowCrates"]);
        }
        else
        {
            TOTAL_CRATES = Convert.ToInt32(slotData["TotalCrates"]);
        }
        
        FIBER_PLANTS = Convert.ToInt32(slotData["FiberPlants"]);
        GLAND_PLANTS = Convert.ToInt32(slotData["GlandPlants"]);
        
        BOXES = Convert.ToInt32(slotData["Boxes"]);
        
        LocationHandler.Initialize();
        
        var fieldDict = typeof(Options)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .ToDictionary(x => x.Name, x => x.GetValue(null));
        
        Plugin.LOGGER.LogInfo("Options:");
        foreach (var field in fieldDict)
            Plugin.LOGGER.LogInfo($"  {field.Key}: {field.Value}");
    }
}