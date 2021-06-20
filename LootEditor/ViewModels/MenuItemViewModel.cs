using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LootEditor.ViewModels
{
    public class MenuItemViewModel : ObservableRecipient
    {
        public string Header { get; }

        public ICommand Command { get; }

        public ObservableCollection<MenuItemViewModel> SubMenuList { get; } = new();

        public MenuItemViewModel(string header, Func<Task> commandAction, Func<bool> canExecute = null)
        {
            Header = header;
            if (canExecute != null)
                Command = new AsyncRelayCommand(commandAction, canExecute);
            else
                Command = new AsyncRelayCommand(commandAction);
        }

        public MenuItemViewModel(string header, Action commandAction, Func<bool> canExecute = null)
        {
            Header = header;
            if (canExecute != null)
                Command = new RelayCommand(commandAction, canExecute);
            else
                Command = new RelayCommand(commandAction);
        }

        public void Add(MenuItemViewModel vm) => SubMenuList.Add(vm);
    }
}
