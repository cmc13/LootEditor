using LootEditor.Models.Enums;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Windows;

namespace LootEditor.ViewModels
{
    public class SalvageCombineViewModel : ObservableRecipient, IAcceptPendingChange
    {
        private Material material;
        private readonly SalvageCombineListViewModel.SalvageObj salvageObj;

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
                        Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(Material)));
                        return;
                    }

                    OnPropertyChanged(nameof(Material));
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
                    OnPropertyChanged(nameof(CombineRange));
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
                    OnPropertyChanged(nameof(CombineValue));
                    OnPropertyChanged(nameof(HasCombineValue));
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
                    OnPropertyChanged(nameof(CombineValue));
                    OnPropertyChanged(nameof(HasCombineValue));
                }
            }
        }

        public event EventHandler<AcceptPendingChangeEventArgs> AcceptPendingChange;
    }
}
