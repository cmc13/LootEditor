using LootEditor.View.ViewModel;
using System;
using System.Windows;
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

        private bool CollectionViewSource_Filter(object item)
        {
            return (item as LootRuleViewModel).Name.IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = DataContext as LootRuleListViewModel;
            CollectionViewSource.GetDefaultView(vm.LootRules).Refresh();
        }

        private void LootEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LootRuleListViewModel;
            CollectionViewSource.GetDefaultView(vm.LootRules).Filter = CollectionViewSource_Filter;
        }
    }
}
