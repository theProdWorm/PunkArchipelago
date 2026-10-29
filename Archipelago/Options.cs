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
    
    public static int SMALL_ENCOUNTER_SHOP_UNLOCKS;
    public static int ADVANCED_ENCOUNTER_SHOP_UNLOCKS;
    public static int FINAL_BOSS_SHOP_UNLOCKS;

    public static int GENERATOR_CACHES;

    public static bool SEPARATE_COLORED_CACHES;
    public static int WHITE_CACHES;
    public static int GREEN_CACHES;
    public static int ORANGE_CACHES;
    public static int PINK_CACHES;
    public static int PURPLE_CACHES;
    public static int YELLOW_CACHES;

    public static int FIBER_PLANTS;
    public static int GLAND_PLANTS;

    public static int LOOT_CRATES;
    
    internal static void SetFrom(Dictionary<string, object> slotData)
    {
        DEATH_LINK_PENALTY = (DeathLinkPenalty) Convert.ToInt32(slotData["DeathLinkPenalty"]);
        DEATH_LINK_DAMAGE = Convert.ToInt32(slotData["DeathLinkDamage"]);
        
        SMALL_ENCOUNTERS = Convert.ToInt32(slotData["SmallEncounters"]);
        ADVANCED_ENCOUNTERS = Convert.ToInt32(slotData["AdvancedEncounters"]);
        FINAL_BOSSES = Convert.ToInt32(slotData["FinalBosses"]);
        
        SMALL_ENCOUNTER_SHOP_UNLOCKS = Convert.ToInt32(slotData["SmallEncounterShopUnlocks"]);
        ADVANCED_ENCOUNTER_SHOP_UNLOCKS = Convert.ToInt32(slotData["AdvancedEncounterShopUnlocks"]);
        FINAL_BOSS_SHOP_UNLOCKS = Convert.ToInt32(slotData["FinalBossShopUnlocks"]);
        
        GENERATOR_CACHES = Convert.ToInt32(slotData["GeneratorCaches"]);

        SEPARATE_COLORED_CACHES = Convert.ToBoolean(slotData["SeparateColoredCaches"]);
        WHITE_CACHES = Convert.ToInt32(slotData["WhiteCaches"]);
        GREEN_CACHES = Convert.ToInt32(slotData["GreenCaches"]);
        ORANGE_CACHES = Convert.ToInt32(slotData["OrangeCaches"]);
        PINK_CACHES = Convert.ToInt32(slotData["PinkCaches"]);
        PURPLE_CACHES = Convert.ToInt32(slotData["PurpleCaches"]);
        YELLOW_CACHES = Convert.ToInt32(slotData["YellowCaches"]);
        
        FIBER_PLANTS = Convert.ToInt32(slotData["FiberPlants"]);
        GLAND_PLANTS = Convert.ToInt32(slotData["GlandPlants"]);
        
        LOOT_CRATES = Convert.ToInt32(slotData["LootCrates"]);
        
        LocationHandler.Initialize();
        
        // var fieldDict = typeof(Options)
        //     .GetFields(BindingFlags.Static | BindingFlags.Public)
        //     .ToDictionary(field => field.Name, field => field.GetValue(null));
        //
        // Plugin.LOGGER.LogInfo("Options:");
        // foreach (var field in fieldDict)
        //     Plugin.LOGGER.LogInfo($"  {field.Key}: {field.Value}");
    }
}