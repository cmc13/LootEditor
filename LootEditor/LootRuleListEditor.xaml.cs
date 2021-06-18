using LootEditor.Models;
using LootEditor.Models.Enums;
using LootEditor.ViewModels;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LootEditor
{
    /// <summary>
    /// Interaction logic for LootRuleListEditor.xaml
    /// </summary>
    public partial class LootRuleListEditor : UserControl
    {
        public LootRuleListEditor()
        {
            InitializeComponent();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            var vm = item.DataContext as LootRuleViewModel;
            vm.ToggleDisabledCommand.Execute(null);
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (txtFilter.Text.StartsWith("has:"))
            {
                // Try to parse criteria filter
                var parts = txtFilter.Text.Split(':', StringSplitOptions.TrimEntries);
                if (parts.Length > 1)
                {
                    if (Enum.TryParse<LootCriteriaType>(parts[1], out var criteriaType))
                    {
                        if (parts.Length == 2)
                        {
                            e.Accepted = e.Item is LootRuleViewModel vmm && vmm.Criteria.Any(c => c.Criteria.Type == criteriaType);
                        }
                        else
                        {
                            e.Accepted = IsItemMatch(parts, criteriaType, e.Item as LootRuleViewModel);
                        }

                        return;
                    }
                }
            }

            var comparison = txtFilter.Text.IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            e.Accepted = e.Item is LootRuleViewModel vm && vm.Name.Contains(txtFilter.Text, comparison);
        }

        private bool IsItemMatch(string[] filter, LootCriteriaType type, LootRuleViewModel vm)
        {
            switch (type)
            {
                case LootCriteriaType.SpellNameMatch:
                    return vm.Criteria.Any(c => c.Criteria.Type == LootCriteriaType.SpellNameMatch && c.Criteria is ValueLootCriteria<string> cc && cc.Value.Contains(filter[2], filter[2].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

                case LootCriteriaType.StringValueMatch:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == LootCriteriaType.StringValueMatch &&
                        c.Criteria is ValueKeyLootCriteria<StringValueKey, string> cc &&
                        (string.IsNullOrEmpty(filter[2]) || (Enum.TryParse<StringValueKey>(filter[2], out var key) && cc.Key == key)) &&
                        (filter.Length == 3 || cc.Value.Contains(filter[3], filter[3].IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)));

                case LootCriteriaType.LongValKeyE:
                case LootCriteriaType.LongValKeyGE:
                case LootCriteriaType.LongValKeyLE:
                case LootCriteriaType.LongValKeyNE:
                case LootCriteriaType.LongValKeyFlagExists:
                case LootCriteriaType.BuffedLongValKeyGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueKeyLootCriteria<LongValueKey, int> cc &&
                        (string.IsNullOrEmpty(filter[2]) || (Enum.TryParse<LongValueKey>(filter[2], out var key) && cc.Key == key)) &&
                        (filter.Length <= 3 || (int.TryParse(filter[3], out var test) && cc.Value == test)));

                case LootCriteriaType.DoubleValKeyGE:
                case LootCriteriaType.DoubleValKeyLE:
                case LootCriteriaType.BuffedDoubleValKeyGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueKeyLootCriteria<DoubleValueKey, int> cc &&
                        (string.IsNullOrEmpty(filter[2]) || (Enum.TryParse<DoubleValueKey>(filter[2], out var key) && cc.Key == key)) &&
                        (filter.Length <= 3 || (double.TryParse(filter[3], out var test) && cc.Value == test)));

                case LootCriteriaType.DamagePercentGE:
                case LootCriteriaType.BuffedMedianDamageGE:
                case LootCriteriaType.BuffedMissileDamageGE:
                case LootCriteriaType.CalcdBuffedTinkedDamageGE:
                case LootCriteriaType.TotalRatingsGE:
                case LootCriteriaType.MinDamageGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueLootCriteria<double> cc &&
                        double.TryParse(filter[2], out var test) &&
                        cc.Value == test);

                case LootCriteriaType.CharacterLevelGE:
                case LootCriteriaType.CharacterLevelLE:
                case LootCriteriaType.CharacterMainPackEmptySlotsGE:
                case LootCriteriaType.SpellCountGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueLootCriteria<int> cc &&
                        int.TryParse(filter[2], out var test) &&
                        cc.Value == test);

                case LootCriteriaType.ObjectClass:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueLootCriteria<ObjectClass> cc &&
                        Enum.TryParse<ObjectClass>(filter[2], out var test) &&
                        cc.Value == test);

                case LootCriteriaType.AnySimilarColor:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ColorLootCriteria cc &&
                        (string.IsNullOrEmpty(filter[2]) || IsColorMatch(filter[2], cc.Color)) &&
                        (filter.Length <= 3 || string.IsNullOrEmpty(filter[3]) || (double.TryParse(filter[3], out var test) && cc.HDiff == test)) &&
                        (filter.Length <= 4 || string.IsNullOrEmpty(filter[4]) || (decimal.TryParse(filter[4], out var test2) && cc.SVDiff == test2)));

                case LootCriteriaType.CalcedBuffedTinkedTargetMeleeGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is CalculatedBuffedTinkedTargetMeleeGELootCriteria cc &&
                        (string.IsNullOrEmpty(filter[2]) || (double.TryParse(filter[2], out var test) && cc.CalculatedBuffedTinkedDamageOverTime == test)) &&
                        (filter.Length <= 3 || string.IsNullOrEmpty(filter[3]) || (double.TryParse(filter[3], out var test2) && cc.BuffedMeleeDefenseBonus == test2)) &&
                        (filter.Length <= 4 || string.IsNullOrEmpty(filter[4]) || (double.TryParse(filter[4], out var test3) && cc.BuffedAttackBonus == test3)));

                case LootCriteriaType.CharacterSkillGE:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is ValueKeyLootCriteria<SkillType, int> cc &&
                        (string.IsNullOrEmpty(filter[2]) || (Enum.TryParse<SkillType>(filter[2], out var test) && cc.Key == test)) &&
                        (filter.Length <= 3 || string.IsNullOrEmpty(filter[3]) || (int.TryParse(filter[3], out var test2) && cc.Value == test2)));

                case LootCriteriaType.CharacterBaseSkill:
                    return vm.Criteria.Any(c =>
                        c.Criteria.Type == type &&
                        c.Criteria is CharacterBaseSkillLootCriteria cc &&
                        (string.IsNullOrEmpty(filter[2]) || (Enum.TryParse<SkillType>(filter[2], out var test) && cc.SkillType == test)) &&
                        (filter.Length <= 3 || string.IsNullOrEmpty(filter[3]) || (int.TryParse(filter[3], out var test2) && cc.MinSkillValue == test2)) &&
                        (filter.Length <= 4 || string.IsNullOrEmpty(filter[4]) || (int.TryParse(filter[4], out var test3) && cc.MaxSkillValue == test3)));

                case LootCriteriaType.SimilarColorArmorType:
                case LootCriteriaType.SlotExactPalette:
                case LootCriteriaType.SlotSimilarColor:
                    break;
            }

            return false;
        }

        private bool IsColorMatch(string v, Color color)
        {
            return false;
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var cvs = Resources["LootRules"] as CollectionViewSource;
            cvs.View.Refresh();
        }
    }
}
