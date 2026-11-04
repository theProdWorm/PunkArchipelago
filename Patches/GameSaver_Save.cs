using System.Linq;
using System.Reflection;
using HarmonyLib;
using Punk.SaveLoad;
using PunkArchipelago.Archipelago;

namespace PunkArchipelago.Patches;

[HarmonyPatch(typeof(GameSaver), nameof(GameSaver.Save), [typeof(string)])]
public class GameSaver_Save
{
    [HarmonyPostfix]
    private static void Save(GameSaver __instance, ref SaveFolder __result)
    {
        ItemHandler.SerializeCollectedItems();
    }
}