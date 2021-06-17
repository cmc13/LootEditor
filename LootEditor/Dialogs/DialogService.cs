using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;

namespace LootEditor.Dialogs
{
    public class DialogService
    {
        public bool? ShowDialog<TViewModel>(string title, TViewModel viewModel) where TViewModel : ObservableRecipient
        {
            var window = new DialogBase() { DataContext = new DialogViewModel<TViewModel>(title, viewModel), Owner = Application.Current.MainWindow };
            return window.ShowDialog();
        }

        public TEnum? ShowEnumDialog<TEnum>(string message, string caption, out bool doForAllItems) where TEnum : struct
        {
            var vm = new EnumDialogViewModel<TEnum>(message, caption);
            var window = new EnumDialog() { DataContext = vm, Owner = Application.Current.MainWindow };
            window.ShowDialog();
            doForAllItems = vm.DoForAllItems;
            return vm.DialogResult;
        }
    }
}
