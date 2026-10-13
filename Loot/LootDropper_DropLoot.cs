using HarmonyLib;

namespace PunkArchipelago.Loot;

[HarmonyPatch(typeof(LootDropper), "DropLoot")]
public class LootDropper_DropLoot
{
    private static void Prefix()
    {
        ArchipelagoPlugin.Logger.LogInfo("DropLoot called");
    }
}