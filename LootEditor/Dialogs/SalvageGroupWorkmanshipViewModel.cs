using CommunityToolkit.Mvvm.ComponentModel;
using LootEditor.Models.Enums;

namespace LootEditor.Dialogs;

public class SalvageGroupWorkmanshipViewModel : ObservableRecipient
{
    private double workmanship = 1;

    public SalvageGroupWorkmanshipViewModel(SalvageGroup selectedSalvageGroup)
    {
        SelectedSalvageGroup = selectedSalvageGroup;
    }

    public string MaterialString => string.Join(", ", SelectedSalvageGroup.GetMaterials());

    public double Workmanship
    {
        get => workmanship;
        set
        {
            if (workmanship != value)
            {
                workmanship = value;
                OnPropertyChanged(nameof(Workmanship));
            }
        }
    }

    public SalvageGroup SelectedSalvageGroup { get; }
}
