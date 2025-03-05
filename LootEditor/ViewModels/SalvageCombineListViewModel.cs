using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LootEditor.Models;
using LootEditor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LootEditor.ViewModels;

public partial class SalvageCombineListViewModel
    : DirtyViewModel
{
    public class SalvageObj : ObservableObject
    {
        private string combineRange;
        private int? combineValue;

        public string Name { get; } = "Salvage Combine";

        public string CombineRange
        {
            get => combineRange;
            set
            {
                if (combineRange != value)
                {
                    combineRange = value;
                    OnPropertyChanged(nameof(CombineRange));
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
                    OnPropertyChanged(nameof(CombineValue));
                    OnPropertyChanged(nameof(HasCombineValue));
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

    private KeyValuePair<Material, SalvageObj>? selectedItem;

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

    [RelayCommand(CanExecute = nameof(AddSalvageCommand_CanExecute))]
    public void AddSalvage()
    {
        var first = new KeyValuePair<Material, SalvageObj>(
            Enum.GetValues(typeof(Material)).Cast<Material>().FirstOrDefault(m => !CombineRules.ContainsKey(m)),
            new SalvageObj() { CombineRange = null, CombineValue = null });
        CombineRules.Add(first);
        SelectedItem = first;
    }

    [RelayCommand(CanExecute = nameof(DeleteSalvageCommand_CanExecute))]
    public void DeleteSalvage()
    {
        if (SelectedItem != null)
            CombineRules.Remove(SelectedItem.Value.Key);
    }

    public KeyValuePair<Material, SalvageObj>? SelectedItem
    {
        get => selectedItem;
        set
        {
            if ((selectedItem == null && value != null) || !selectedItem.Equals(value))
            {
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                DeleteSalvageCommand.NotifyCanExecuteChanged();

                if (selectedItem.HasValue)
                    SalvageCombineViewModel = new SalvageCombineViewModel(selectedItem.Value.Key, selectedItem.Value.Value);
                else
                    SalvageCombineViewModel = null;
            }
        }
    }

    [ObservableProperty]
    private SalvageCombineViewModel salvageCombineViewModel;

    partial void OnSalvageCombineViewModelChanging(SalvageCombineViewModel oldValue, SalvageCombineViewModel newValue)
    {
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= Vm_PropertyChanged;
            oldValue.AcceptPendingChange -= Vm_AcceptPendingChange;
        }
    }

    partial void OnSalvageCombineViewModelChanged(SalvageCombineViewModel oldValue, SalvageCombineViewModel newValue)
    {
        if (newValue != null)
        {
            newValue.PropertyChanged += Vm_PropertyChanged;
            newValue.AcceptPendingChange += Vm_AcceptPendingChange;
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
                OnPropertyChanged(nameof(DefaultCombineRange));
                IsDirty = true;
            }
        }
    }

    public bool AddSalvageCommand_CanExecute() => Enum.GetValues(typeof(Material)).Cast<Material>().Any(m => !CombineRules.ContainsKey(m));

    public bool DeleteSalvageCommand_CanExecute() => SelectedItem.HasValue;

    public ObservableDictionary<Material, SalvageObj> CombineRules { get; } = [];

    public void Clean() => IsDirty = false;

    private void Vm_AcceptPendingChange(object sender, AcceptPendingChangeEventArgs e)
    {
        if (e.PropertyName == nameof(SalvageCombineViewModel.Material))
        {
            if (CombineRules.ContainsKey((Material)e.NewValue))
            {
                e.Cancel = true;
                SelectedItem = CombineRules.First(kv => kv.Key == (Material)e.NewValue);
            }
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

            AddSalvageCommand.NotifyCanExecuteChanged();
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

            AddSalvageCommand.NotifyCanExecuteChanged();
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
        {
            salvageCombineBlock.Materials.Clear();
            salvageCombineBlock.MaterialValues.Clear();
        }

        IsDirty = true;
    }
}
