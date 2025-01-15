using LootEditor.Models.Enums;
using LootEditor.ViewModels;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
                        e.Accepted = e.Item is LootRuleViewModel vmm && vmm.Criteria.Any(c => c.Criteria.IsMatch(parts));
                        return;
                    }
                }
            }

            var comparison = txtFilter.Text.IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            e.Accepted = e.Item is LootRuleViewModel vm && vm.Name.Contains(txtFilter.Text, comparison);
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var cvs = Resources["LootRules"] as CollectionViewSource;
            cvs.View.Refresh();
        }
    }
}
