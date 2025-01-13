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
    }
}
