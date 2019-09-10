using GalaSoft.MvvmLight;
using LootEditor.Model.Enums;

namespace LootEditor.View.ViewModel
{
    public class SalvageCombineViewModel : ViewModelBase
    {
        private Material material;
        private SalvageCombineListViewModel.SalvageObj salvageObj;

        public SalvageCombineViewModel(Material material, SalvageCombineListViewModel.SalvageObj salvageObj)
        {
            this.material = material;
            this.salvageObj = salvageObj;
        }

        public Material Material
        {
            get => material;
            set
            {
                if (material != value)
                {
                    material = value;
                    RaisePropertyChanged(nameof(Material));
                }
            }
        }

        public string CombineRange
        {
            get => salvageObj.CombineRange;
            set
            {
                if (salvageObj.CombineRange != value)
                {
                    salvageObj.CombineRange = value;
                    RaisePropertyChanged(nameof(CombineRange));
                }
            }
        }

        public int? CombineValue
        {
            get => salvageObj.CombineValue;
            set
            {
                if (salvageObj.CombineValue != value)
                {
                    salvageObj.CombineRange = value;
                    RaisePropertyChanged(nameof(CombineValue));
                }
            }
        }
    }
}
