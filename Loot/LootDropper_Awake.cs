using System.Linq;
using HarmonyLib;
using MyBox;
using WeightedDistribution;

namespace PunkArchipelago.Loot;

[HarmonyPatch(typeof(LootDropper), "Awake")]
public class LootDropper_Awake
{
    private static readonly AccessTools.StructFieldRef<DropTableItem, DropTableItemCountSource> _countSource =
        AccessTools.StructFieldRefAccess<DropTableItem, DropTableItemCountSource>("countSource");
    private static readonly AccessTools.StructFieldRef<DropTableItem, bool> _useGroup =
        AccessTools.StructFieldRefAccess<DropTableItem, bool>("useGroup");
    private static readonly AccessTools.StructFieldRef<DropTableItem, DropTableWeightedGroup> _group =
        AccessTools.StructFieldRefAccess<DropTableItem, DropTableWeightedGroup>("group");
    private static readonly AccessTools.StructFieldRef<DropTableItem, MinMaxInt> _count =
        AccessTools.StructFieldRefAccess<DropTableItem, MinMaxInt>("count");
    private static readonly AccessTools.StructFieldRef<DropTableItem, DroppabbleItem> _item =
        AccessTools.StructFieldRefAccess<DropTableItem, DroppabbleItem>("item");
    private static readonly AccessTools.StructFieldRef<DropTableItem, float> _probability =
        AccessTools.StructFieldRefAccess<DropTableItem, float>("probability");
    
    private static void Prefix(LootDropper __instance)
    {
        if (false)
        {
            string message = $"LootDropper \"{__instance.name}\" drops:\n";
            for (int i = 0; i < __instance.dropTable.items.Count; i++)
            {
                var dropTableItem = __instance.dropTable.items[i];

                if (_countSource(ref dropTableItem) == DropTableItemCountSource.MinMax)
                {
                    int min = _count(ref dropTableItem).Min;
                    int max = _count(ref dropTableItem).Max;

                    if (_useGroup(ref dropTableItem))
                    {
                        var groupItems = string.Join(", ", _group(ref dropTableItem).itemDistribution.Items
                            .Select(x => x as DistributionItem<DroppabbleItem>)
                            .Select(x => x.Value)
                            .Select(x => GetItemName(ref x)));

                        message += $"  {min}-{max} items among: {groupItems}";
                    }
                    else
                    {
                        message += $"  {min}-{max} {GetItemName(ref _item(ref dropTableItem))}";
                    }
                }
                else if (_countSource(ref dropTableItem) == DropTableItemCountSource.Probability)
                {
                    float chance = _probability(ref dropTableItem) * 100;

                    if (_useGroup(ref dropTableItem))
                    {
                        var groupItems = string.Join(", ", _group(ref dropTableItem).itemDistribution.Items
                            .Select(x => x as DistributionItem<DroppabbleItem>)
                            .Select(x => x.Value)
                            .Select(x => GetItemName(ref x)));

                        message += $"  One of {groupItems} with a {chance}% chance";
                    }
                    else
                    {
                        message += $"  {GetItemName(ref _item(ref dropTableItem))} with a {chance}% chance";
                    }
                }

                message += "\n";
            }

            message = message.Trim();
            string[] lines = message.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Contains("Resource") || lines[i].Contains("Corpse"))
                    continue;

                Plugin.LOGGER.LogInfo(message);
                break;
            }
        }
    }

    private static string GetItemName(ref DroppabbleItem item) => item.droppableType switch
    {
        DroppabbleType.Prefab => item.prefab.name,
        DroppabbleType.Module => item.module.displayName,
        DroppabbleType.Ingedient => item.ingredient.displayName,
        DroppabbleType.Consumable => item.consumable.displayName,
        _ => ""
    };
}