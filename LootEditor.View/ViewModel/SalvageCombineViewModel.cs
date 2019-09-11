using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using LootEditor.Model.Enums;

namespace LootEditor.View.ViewModel
{
    public class SalvageCombineViewModel : ViewModelBase, IAcceptPendingChange
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
                    if (this.RaiseAcceptPendingChange(nameof(Material), material, value))
                        material = value;
                    else
                    {
                        DispatcherHelper.RunAsync(() => RaisePropertyChanged(nameof(Material)));
                        return;
                    }

                    RaisePropertyChanged(nameof(Material));
                }
            }
        }

        private bool RaiseAcceptPendingChange(string propertyName, object oldValue, object newValue)
        {
            var e = new AcceptPendingChangeEventArgs(propertyName, oldValue, newValue);
            AcceptPendingChange?.Invoke(this, e);
            return !e.Cancel;
        }

        public string CombineRange
        {
            get => salvageObj?.CombineRange;
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
            get => salvageObj?.CombineValue;
            set
            {
                if (salvageObj.CombineValue != value)
                {
                    salvageObj.CombineValue = value;
                    RaisePropertyChanged(nameof(CombineValue));
                    RaisePropertyChanged(nameof(HasCombineValue));
                }
            }
        }

        public bool HasCombineValue
        {
            get => salvageObj?.HasCombineValue ?? false;
            set
            {
                if (salvageObj.HasCombineValue != value)
                {
                    salvageObj.HasCombineValue = value;
                    RaisePropertyChanged(nameof(CombineValue));
                    RaisePropertyChanged(nameof(HasCombineValue));
                }
            }
        }

        public event EventHandler<AcceptPendingChangeEventArgs> AcceptPendingChange;
    }
}
