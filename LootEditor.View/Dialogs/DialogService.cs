using GalaSoft.MvvmLight;

namespace LootEditor.View.Dialogs
{
    public class DialogService
    {
        public bool? ShowDialog<TViewModel>(string title, TViewModel viewModel) where TViewModel : ViewModelBase
        {
            var window = new DialogBase() { DataContext = new DialogViewModel<TViewModel>(title, viewModel) };
            return window.ShowDialog();
        }
    }
}
