using GalaSoft.MvvmLight;
using System.Windows;

namespace LootEditor.View.Dialogs
{
    public class DialogViewModel<TViewModel> : ViewModelBase where TViewModel : ViewModelBase
    {
        public DialogViewModel(string title, TViewModel childViewModel)
        {
            Title = title;
            ChildViewModel = childViewModel;
        }

        public string Title { get; }
        public TViewModel ChildViewModel { get; }
    }
}
