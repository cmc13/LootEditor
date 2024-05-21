using CommunityToolkit.Mvvm.ComponentModel;

namespace LootEditor.Dialogs;

public class DialogViewModel<TViewModel> : ObservableRecipient where TViewModel : ObservableRecipient
{
    public DialogViewModel(string title, TViewModel childViewModel)
    {
        Title = title;
        ChildViewModel = childViewModel;
    }

    public string Title { get; }
    public TViewModel ChildViewModel { get; }
}
