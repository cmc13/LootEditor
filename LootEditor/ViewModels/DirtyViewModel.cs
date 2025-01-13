using CommunityToolkit.Mvvm.ComponentModel;

namespace LootEditor.ViewModels;

public partial class DirtyViewModel
    : ObservableRecipient
{
    private bool isDirty = false;

    public virtual bool IsDirty
    {
        get => isDirty;
        set
        {
            if (isDirty != value)
            {
                isDirty = value;
                OnPropertyChanged(nameof(IsDirty));
            }
        }
    }
}
