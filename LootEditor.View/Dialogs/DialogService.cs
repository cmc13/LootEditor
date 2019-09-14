namespace LootEditor.View.Dialogs
{
    public class DialogService
    {
        public bool? ShowDialog<TViewModel>(string title, TViewModel viewModel)
        {
            var window = new DialogBase() { DataContext = new DialogViewModel<TViewModel>(title, viewModel) };
            return window.ShowDialog();
        }
    }
}
