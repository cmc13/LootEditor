using LootEditor.View.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace LootEditor.View
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
            var comparison = txtFilter.Text.IsLower() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            e.Accepted = e.Item is LootRuleViewModel vm && vm.Name.IndexOf(txtFilter.Text, comparison) >= 0;
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var cvs = Resources["LootRules"] as CollectionViewSource;
            cvs.View.Refresh();
        }
    }
}
