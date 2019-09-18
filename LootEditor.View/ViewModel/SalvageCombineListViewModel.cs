using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LootEditor.Model;
using LootEditor.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LootEditor.View.ViewModel
{
    public class SalvageCombineListViewModel : ViewModelBase
    {
        public class SalvageObj : ObservableObject
        {
            private string combineRange;
            private int? combineValue;

            public string CombineRange
            {
                get => combineRange;
                set
                {
                    if (combineRange != value)
                    {
                        combineRange = value;
                        RaisePropertyChanged(nameof(CombineRange));
                    }
                }
            }

            public int? CombineValue
            {
                get => combineValue;
                set
                {
                    if (combineValue != value)
                    {
                        combineValue = value;
                        RaisePropertyChanged(nameof(CombineValue));
                        RaisePropertyChanged(nameof(HasCombineValue));
                    }
                }
            }

            public bool HasCombineValue
            {
                get => CombineValue.HasValue;
                set
                {
                    if (CombineValue.HasValue != value)
                    {
                        CombineValue = value ? (int?)0 : null;
                    }
                }
            }
        }

        private readonly SalvageCombineBlockType salvageCombineBlock;
        private bool isDirty = false;
        private KeyValuePair<Material, SalvageObj>? selectedItem;
        private SalvageCombineViewModel currentSalvageCombineViewModel;

        public SalvageCombineListViewModel(LootFile lootFile)
        {
            salvageCombineBlock = lootFile.ExtraBlocks.SingleOrDefault(b => b is SalvageCombineBlockType) as SalvageCombineBlockType;

            if (salvageCombineBlock != null)
            {
                foreach (var item in salvageCombineBlock.Materials)
                {
                    if (!CombineRules.TryGetValue(item.Key, out var salvageObj))
                    {
                        salvageObj = new SalvageObj();
                        CombineRules.Add(item.Key, salvageObj);
                    }

                    salvageObj.CombineRange = item.Value;
                }

                if (salvageCombineBlock.MaterialValues != null)
                {
                    foreach (var item in salvageCombineBlock.MaterialValues)
                    {
                        if (!CombineRules.TryGetValue(item.Key, out var salvageObj))
                        {
                            salvageObj = new SalvageObj();
                            CombineRules.Add(item.Key, salvageObj);
                        }

                        salvageObj.CombineValue = item.Value;
                    }
                }
            }

            CombineRules.CollectionChanged += CombineRules_CollectionChanged;
        }

        public RelayCommand AddSalvageCommand => new RelayCommand(() =>
        {
            var first = new KeyValuePair<Material, SalvageObj>(
                Enum.GetValues(typeof(Material)).Cast<Material>().FirstOrDefault(m => !CombineRules.ContainsKey(m)),
                new SalvageObj() { CombineRange = null, CombineValue = null });
            CombineRules.Add(first);
            SelectedItem = first;
        }, () => Enum.GetValues(typeof(Material)).Cast<Material>().Any(m => !CombineRules.ContainsKey(m)));

        public RelayCommand DeleteSalvageCommand => new RelayCommand(() =>
        {
            CombineRules.Remove(SelectedItem.Value.Key);
        }, () => SelectedItem.HasValue);

        public KeyValuePair<Material, SalvageObj>? SelectedItem
        {
            get => selectedItem;
            set
            {
                if (!selectedItem.Equals(value))
                {
                    selectedItem = value;
                    RaisePropertyChanged(nameof(SelectedItem));

                    if (selectedItem.HasValue)
                        SalvageCombineViewModel = new SalvageCombineViewModel(selectedItem.Value.Key, selectedItem.Value.Value);
                    else
                        SalvageCombineViewModel = null;
                }
            }
        }

        public SalvageCombineViewModel SalvageCombineViewModel
        {
            get => currentSalvageCombineViewModel;
            set
            {
                if (currentSalvageCombineViewModel != value)
                {
                    if (currentSalvageCombineViewModel != null)
                    {
                        currentSalvageCombineViewModel.PropertyChanged -= Vm_PropertyChanged;
                        currentSalvageCombineViewModel.AcceptPendingChange -= Vm_AcceptPendingChange;
                    }

                    currentSalvageCombineViewModel = value;

                    if (currentSalvageCombineViewModel != null)
                    {
                        currentSalvageCombineViewModel.PropertyChanged += Vm_PropertyChanged;
                        currentSalvageCombineViewModel.AcceptPendingChange += Vm_AcceptPendingChange;
                    }

                    RaisePropertyChanged(nameof(SalvageCombineViewModel));
                }
            }
        }

        public bool IsDirty
        {
            get => isDirty;
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public string DefaultCombineRange
        {
            get => salvageCombineBlock?.DefaultCombineString;
            set
            {
                if (salvageCombineBlock.DefaultCombineString != value)
                {
                    salvageCombineBlock.DefaultCombineString = value;
                    RaisePropertyChanged(nameof(DefaultCombineRange));
                    IsDirty = true;
                }
            }
        }

        public ObservableDictionary<Material, SalvageObj> CombineRules { get; } = new ObservableDictionary<Material, SalvageObj>();

        public void Clean()
        {
            IsDirty = false;
        }

        private void Vm_AcceptPendingChange(object sender, AcceptPendingChangeEventArgs e)
        {
            if (e.PropertyName == nameof(SalvageCombineViewModel.Material))
            {
                if (salvageCombineBlock.Materials.ContainsKey((Material)e.NewValue) || salvageCombineBlock.MaterialValues.ContainsKey((Material)e.NewValue))
                    e.Cancel = true;
                else
                {
                    SelectedItem = null;
                    CombineRules.Add((Material)e.NewValue, CombineRules[(Material)e.OldValue]);
                    CombineRules.Remove((Material)e.OldValue);

                    if (salvageCombineBlock.Materials.ContainsKey((Material)e.OldValue))
                    {
                        salvageCombineBlock.Materials[(Material)e.NewValue] = salvageCombineBlock.Materials[(Material)e.OldValue];
                        salvageCombineBlock.Materials.Remove((Material)e.OldValue);
                    }

                    if (salvageCombineBlock.MaterialValues.ContainsKey((Material)e.OldValue))
                    {
                        salvageCombineBlock.MaterialValues[(Material)e.NewValue] = salvageCombineBlock.MaterialValues[(Material)e.OldValue];
                        salvageCombineBlock.MaterialValues.Remove((Material)e.OldValue);
                    }

                    IsDirty = true;
                }
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = sender as SalvageCombineViewModel;
            if (e.PropertyName == nameof(SalvageCombineViewModel.CombineRange))
            {
                if (string.IsNullOrEmpty(vm.CombineRange) || vm.CombineRange == salvageCombineBlock.DefaultCombineString)
                    salvageCombineBlock.Materials.Remove(vm.Material);
                else
                    salvageCombineBlock.Materials[vm.Material] = vm.CombineRange;
            }
            else if (e.PropertyName == nameof(SalvageCombineViewModel.CombineValue))
            {
                if (vm.CombineValue == null)
                    salvageCombineBlock.MaterialValues.Remove(vm.Material);
                else
                    salvageCombineBlock.MaterialValues[vm.Material] = vm.CombineValue.Value;
            }
            IsDirty = true;
        }

        private void CombineRules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<Material, SalvageObj> item in e.NewItems)
                {
                    if (item.Value.CombineRange != null)
                        salvageCombineBlock.Materials.Add(item.Key, item.Value.CombineRange);
                    if (item.Value.HasCombineValue)
                        salvageCombineBlock.MaterialValues.Add(item.Key, item.Value.CombineValue.Value);
                }

                AddSalvageCommand?.RaiseCanExecuteChanged();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (KeyValuePair<Material, SalvageObj> item in e.OldItems)
                {
                    if (item.Value.CombineRange != null)
                        salvageCombineBlock.Materials.Remove(item.Key);
                    if (item.Value.HasCombineValue)
                        salvageCombineBlock.MaterialValues.Remove(item.Key);
                }

                AddSalvageCommand?.RaiseCanExecuteChanged();
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                salvageCombineBlock.Materials.Clear();
                salvageCombineBlock.MaterialValues.Clear();
            }

            IsDirty = true;
        }
    }
}
