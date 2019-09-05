using LootEditor.View.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LootEditor.View
{
    public class LootCriteriaDataTemplateSelector : DataTemplateSelector
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
            var criteria = item as LootCriteriaViewModel;
            if (criteria != null)
            {
                switch (criteria.Type)
                {
                    case Model.LootCriteriaType.AnySimilarColor: return ColorLootCriteriaTemplate;
                    case Model.LootCriteriaType.DisabledRule: return DisabledRuleTemplate;
                    case Model.LootCriteriaType.BuffedLongValKeyGE:
                    case Model.LootCriteriaType.LongValKeyFlagExists:
                    case Model.LootCriteriaType.LongValKeyGE:
                    case Model.LootCriteriaType.LongValKeyNE:
                    case Model.LootCriteriaType.LongValKeyLE:
                    case Model.LootCriteriaType.LongValKeyE: return LongValueKeyTemplate;
                    case Model.LootCriteriaType.BuffedDoubleValKeyGE:
                    case Model.LootCriteriaType.DoubleValKeyGE:
                    case Model.LootCriteriaType.DoubleValKeyLE: return DoubleValueKeyTemplate;
                    case Model.LootCriteriaType.StringValueMatch: return StringValueKeyTemplate;
                    case Model.LootCriteriaType.BuffedMedianDamageGE:
                    case Model.LootCriteriaType.BuffedMissileDamageGE:
                    case Model.LootCriteriaType.CalcdBuffedTinkedDamageGE:
                    case Model.LootCriteriaType.CharacterLevelGE:
                    case Model.LootCriteriaType.CharacterLevelLE:
                    case Model.LootCriteriaType.CharacterMainPackEmptySlotsGE:
                    case Model.LootCriteriaType.DamagePercentGE:
                    case Model.LootCriteriaType.MinDamageGE:
                    case Model.LootCriteriaType.SpellCountGE:
                    case Model.LootCriteriaType.SpellNameMatch:
                    case Model.LootCriteriaType.TotalRatingsGE: return ValueTemplate;
                    case Model.LootCriteriaType.ObjectClass: return ObjectClassTemplate;
                    case Model.LootCriteriaType.SpellMatch: return SpellMatchTemplate;
                    case Model.LootCriteriaType.CharacterBaseSkill: return CharacterBaseSkillTemplate;
                    case Model.LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE: return CalcedBuffedTinkedTargetMeleeGETemplate;
                    case Model.LootCriteriaType.SlotExactPalette: return SlotExactPaletteTemplate;
                    case Model.LootCriteriaType.CharacterSkillGE: return CharacterSkillGETemplate;
                    case Model.LootCriteriaType.SlotSimilarColor: return SlotSimilarColorTemplate;
                    case Model.LootCriteriaType.SimilarColorArmorType: return SimilarColorArmorTypeTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
