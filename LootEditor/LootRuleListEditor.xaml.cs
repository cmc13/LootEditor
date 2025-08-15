using LootEditor.Models;
using LootEditor.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace LootEditor;

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
        if (e.Item is not LootRuleViewModel vm)
        {
            e.Accepted = false;
            return;
        }

        try
        {
            var filters = new Services.FilterParser(txtFilter.Text).ParseAll();
            if (filters.Count > 0)
            {
                e.Accepted = filters.All(filter =>
                {
                    var match = filter.Type != CriteriaFilterType.Unstructured
                        ? vm.Criteria.Any(c => c.Criteria.IsMatch(filter.Tokens))
                        : vm.Name?.IndexOf(filter.Tokens[0], filter.Tokens[0].Any(char.IsUpper) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) >= 0;

                    return filter.IsNegated ? !match : match;
                });

                return;
            }
        }
        catch { }

        // No filters? Accept all
        e.Accepted = true;
    }

    private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        var cvs = Resources["LootRules"] as CollectionViewSource;
        cvs.View.Refresh();
    }
}
