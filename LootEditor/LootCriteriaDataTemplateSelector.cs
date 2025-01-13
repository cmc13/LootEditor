using LootEditor.Models.Enums;
using LootEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace LootEditor;

public sealed class LootCriteriaDataTemplateSelector
    : DataTemplateSelector
{
    public DataTemplate ColorLootCriteriaTemplate { get; set; }
    public DataTemplate DisabledRuleTemplate { get; set; }
    public DataTemplate LongValueKeyTemplate { get; set; }
    public DataTemplate DoubleValueKeyTemplate { get; set; }
    public DataTemplate StringValueKeyTemplate { get; set; }
    public DataTemplate ValueTemplate { get; set; }
    public DataTemplate ObjectClassTemplate { get; set; }
    public DataTemplate SpellMatchTemplate { get; set; }
    public DataTemplate CharacterBaseSkillTemplate { get; set; }
    public DataTemplate CalcedBuffedTinkedTargetMeleeGETemplate { get; set; }
    public DataTemplate SlotExactPaletteTemplate { get; set; }
    public DataTemplate CharacterSkillGETemplate { get; set; }
    public DataTemplate SlotSimilarColorTemplate { get; set; }
    public DataTemplate SimilarColorArmorTypeTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is LootCriteriaViewModel criteria)
        {
            switch (criteria.Type)
            {
                case LootCriteriaType.AnySimilarColor: return ColorLootCriteriaTemplate;
                case LootCriteriaType.DisabledRule: return DisabledRuleTemplate;
                case LootCriteriaType.BuffedLongValKeyGE:
                case LootCriteriaType.LongValKeyFlagExists:
                case LootCriteriaType.LongValKeyGE:
                case LootCriteriaType.LongValKeyNE:
                case LootCriteriaType.LongValKeyLE:
                case LootCriteriaType.LongValKeyE: return LongValueKeyTemplate;
                case LootCriteriaType.BuffedDoubleValKeyGE:
                case LootCriteriaType.DoubleValKeyGE:
                case LootCriteriaType.DoubleValKeyLE: return DoubleValueKeyTemplate;
                case LootCriteriaType.StringValueMatch: return StringValueKeyTemplate;
                case LootCriteriaType.BuffedMedianDamageGE:
                case LootCriteriaType.BuffedMissileDamageGE:
                case LootCriteriaType.CalcdBuffedTinkedDamageGE:
                case LootCriteriaType.CharacterLevelGE:
                case LootCriteriaType.CharacterLevelLE:
                case LootCriteriaType.CharacterMainPackEmptySlotsGE:
                case LootCriteriaType.DamagePercentGE:
                case LootCriteriaType.MinDamageGE:
                case LootCriteriaType.SpellCountGE:
                case LootCriteriaType.SpellNameMatch:
                case LootCriteriaType.TotalRatingsGE: return ValueTemplate;
                case LootCriteriaType.ObjectClass: return ObjectClassTemplate;
                case LootCriteriaType.SpellMatch: return SpellMatchTemplate;
                case LootCriteriaType.CharacterBaseSkill: return CharacterBaseSkillTemplate;
                case LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE: return CalcedBuffedTinkedTargetMeleeGETemplate;
                case LootCriteriaType.SlotExactPalette: return SlotExactPaletteTemplate;
                case LootCriteriaType.CharacterSkillGE: return CharacterSkillGETemplate;
                case LootCriteriaType.SlotSimilarColor: return SlotSimilarColorTemplate;
                case LootCriteriaType.SimilarColorArmorType: return SimilarColorArmorTypeTemplate;
            }
        }

        return base.SelectTemplate(item, container);
    }
}
