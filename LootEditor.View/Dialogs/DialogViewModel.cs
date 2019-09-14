using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.View.Dialogs
{
    public class DialogViewModel<TViewModel> : ViewModelBase
    {
        public DialogViewModel(string title, TViewModel childViewModel)
        {
            Title = title;
            ChildViewModel = childViewModel;
        }

        public string Title { get; }
        public TViewModel ChildViewModel { get; }

        public MessageBoxResult Result { get; set; }
    }
}
