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
    
    public static int FINAL_BOSSES;

    public static bool ENCOUNTERS_UNLOCK_SHOP_ITEMS;
    
    public static bool SEPARATE_COLORED_CRATES;

    internal static void SetFrom(Dictionary<string, object> slotData)
    {
        DEATH_LINK_PENALTY = (DeathLinkPenalty) Convert.ToInt32(slotData["DeathLinkPenalty"]);
        DEATH_LINK_DAMAGE = Convert.ToInt32(slotData["DeathLinkDamage"]);
        
        FINAL_BOSSES = Convert.ToInt32(slotData["FinalBosses"]);
        
        ENCOUNTERS_UNLOCK_SHOP_ITEMS = Convert.ToBoolean(slotData["EncountersUnlockShopItems"]);
        
        SEPARATE_COLORED_CRATES = Convert.ToBoolean(slotData["SeparateColoredCrates"]);
        
        // var fieldDict = typeof(Options)
        //     .GetFields(BindingFlags.Public | BindingFlags.Static)
        //     .ToDictionary(x => x.Name, x => x.GetValue(null));
        //
        // Plugin.LOGGER.LogInfo("Options:");
        // foreach (var field in fieldDict)
        //     Plugin.LOGGER.LogInfo($"  {field.Key}: {field.Value}");
    }
}