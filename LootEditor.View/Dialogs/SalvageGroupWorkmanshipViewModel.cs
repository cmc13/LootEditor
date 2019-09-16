using GalaSoft.MvvmLight;
using LootEditor.Model.Enums;

namespace LootEditor.View.Dialogs
{
    public class SalvageGroupWorkmanshipViewModel : ViewModelBase
    {
        private double workmanship = 1;

        public SalvageGroupWorkmanshipViewModel(SalvageGroup selectedSalvageGroup)
        {
            SelectedSalvageGroup = selectedSalvageGroup;
        }

        public double Workmanship
        {
            get => workmanship;
            set
            {
                if (workmanship != value)
                {
                    workmanship = value;
                    RaisePropertyChanged(nameof(Workmanship));
                }
            }
        }

        public SalvageGroup SelectedSalvageGroup { get; }
    }
}
