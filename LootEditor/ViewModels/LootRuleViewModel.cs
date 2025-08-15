using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Models;
using LootEditor.Models.Enums;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LootEditor.ViewModels;

public partial class LootRuleViewModel : DirtyViewModel, IDropTarget
{
    [ObservableProperty]
    private LootCriteriaViewModel selectedCriteria;

    partial void OnSelectedCriteriaChanged(LootCriteriaViewModel value)
    {
        CloneCriteriaCommand?.NotifyCanExecuteChanged();
        RemoveCriteriaCommand?.NotifyCanExecuteChanged();
        CutItemCommand?.NotifyCanExecuteChanged();
        CopyItemCommand?.NotifyCanExecuteChanged();
        FilterMatchingRulesCommand?.NotifyCanExecuteChanged();
    }

    public override bool IsDirty
    {
        get => base.IsDirty || Criteria.Any(c => c.IsDirty);
        set => base.IsDirty = value;
    }

    public string Name
    {
        get => Rule.Name;
        set
        {
            if (Rule.Name != value)
            {
                Rule.Name = value;
                OnPropertyChanged(nameof(Name));
                IsDirty = true;
            }
        }
    }

    public LootAction Action
    {
        get => Rule.Action;
        set
        {
            if (Rule.Action != value)
            {
                Rule.Action = value;
                OnPropertyChanged(nameof(Action));
                IsDirty = true;
            }
        }
    }

    public int KeepUpToCount
    {
        get => Rule.KeepUpToCount;
        set
        {
            if (Rule.KeepUpToCount != value)
            {
                Rule.KeepUpToCount = value;
                OnPropertyChanged(nameof(KeepUpToCount));
                IsDirty = true;
            }
        }
    }

    public LootRule Rule { get; }

    public bool IsDisabled => Rule.Criteria.Any(c => c.Type == LootCriteriaType.DisabledRule && ((ValueLootCriteria<bool>)c).Value == true);

    public ObservableCollection<LootCriteriaViewModel> Criteria { get; } = [];

    public LootRuleViewModel(LootRule rule)
    {
        this.Rule = rule;
        Criteria.Clear();
        foreach (var crit in rule.Criteria)
        {
            var vm = LootCriteriaViewModelFactory.CreateViewModel(crit);
            vm.PropertyChanged += Vm_PropertyChanged;
            Criteria.Add(vm);
        }

        Criteria.CollectionChanged += Criteria_CollectionChanged;
    }

    private void Criteria_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Criteria));
    }

    private bool CanPaste() => Clipboard.ContainsData(nameof(LootCriteria));

    [RelayCommand(CanExecute = nameof(CanPaste))]
    private void PasteItem()
    {
        var data = Clipboard.GetData(nameof(LootCriteria)) as LootCriteria;

        var newCriteria = data.Clone() as LootCriteria;
        AddCriteria(newCriteria);
    }

    [RelayCommand(CanExecute = nameof(SelectedCriteria_CanExecute))]
    private void CopyItem()
    {
        Clipboard.SetData(nameof(LootCriteria), SelectedCriteria.Criteria);
        PasteItemCommand?.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(SelectedCriteria_CanExecute))]
    private void CutItem()
    {
        Clipboard.SetData(nameof(LootCriteria), SelectedCriteria.Criteria);
        RemoveCriteriaCommand.Execute(null);
        PasteItemCommand?.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddCriteria()
    {
        var newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.AnySimilarColor);
        AddCriteria(newCriteria);
    }

    [RelayCommand]
    private void ToggleDisabled()
    {
        if (IsDisabled)
        {
            foreach (var crit in Criteria.Where(c => c.Type == LootCriteriaType.DisabledRule).ToList())
            {
                crit.PropertyChanged -= Vm_PropertyChanged;
                Rule.RemoveCriteria(crit.Criteria);
                Criteria.Remove(crit);
            }
        }
        else
        {
            var disabledRule = LootCriteria.CreateLootCriteria(LootCriteriaType.DisabledRule) as ValueLootCriteria<bool>;
            disabledRule.Value = true;
            Rule.AddCriteria(disabledRule);
            var vm = LootCriteriaViewModelFactory.CreateViewModel(disabledRule);
            vm.PropertyChanged += Vm_PropertyChanged;
            Criteria.Add(vm);
        }

        IsDirty = true;
        OnPropertyChanged(nameof(IsDisabled));
    }

    [RelayCommand(CanExecute = nameof(SelectedCriteria_CanExecute))]
    private void CloneCriteria()
    {
        var sel = SelectedCriteria;
        if (sel != null)
        {
            var newCriteria = sel.Criteria.Clone() as LootCriteria;
            AddCriteria(newCriteria);
        }
    }

    [RelayCommand(CanExecute = nameof(SelectedCriteria_CanExecute))]
    private void RemoveCriteria()
    {
        var sel = SelectedCriteria;
        if (sel != null)
        {
            sel.PropertyChanged -= Vm_PropertyChanged;
            Rule.RemoveCriteria(sel.Criteria);
            Criteria.Remove(sel);
            IsDirty = true;

            if (sel.Type == LootCriteriaType.DisabledRule)
                OnPropertyChanged(nameof(IsDisabled));
        }
    }

    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var v = sender as LootCriteriaViewModel;
        if (e.PropertyName == nameof(LootCriteriaViewModel.Type))
        {
            SelectedCriteria = null;

            v.PropertyChanged -= Vm_PropertyChanged;
            var idx = Criteria.IndexOf(v);
            Rule.RemoveCriteria(v.Criteria);
            Criteria.RemoveAt(idx);

            // property type was changed, need to generate new VM
            var newCriteria = LootCriteria.CreateLootCriteria(v.Type);
            var vm = LootCriteriaViewModelFactory.CreateViewModel(newCriteria);
            vm.PropertyChanged += Vm_PropertyChanged;

            Rule.AddCriteria(newCriteria, idx);
            Criteria.Insert(idx, vm);

            IsDirty = true;

            if (newCriteria.Type == LootCriteriaType.DisabledRule)
                OnPropertyChanged(nameof(IsDisabled));

            SelectedCriteria = vm;
        }
        else if (v.Type == LootCriteriaType.DisabledRule)
            OnPropertyChanged(nameof(IsDisabled));
        OnPropertyChanged(nameof(IsDirty));
    }

    [RelayCommand(CanExecute = nameof(FilterMatchingRules_CanExecute))]
    private void FilterMatchingRules(LootRuleListViewModel lootRuleListViewModel)
    {
        // Generate filter
        var filter = SelectedCriteria.Criteria.Filter;

        // Send to MainVM
        lootRuleListViewModel.Filter = filter;
    }

    private bool FilterMatchingRules_CanExecute(LootRuleListViewModel _) => SelectedCriteria_CanExecute();

    private bool SelectedCriteria_CanExecute() => SelectedCriteria != null;

    public void Clean()
    {
        foreach (var criteria in Criteria)
            criteria.Clean();
        IsDirty = false;
    }

    public LootRule CloneRule() => Rule.Clone() as LootRule;

    void IDropTarget.DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is LootCriteriaViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    void IDropTarget.Drop(IDropInfo dropInfo)
    {
        if (dropInfo.InsertIndex != dropInfo.DragInfo.SourceIndex)
        {
            if (dropInfo.InsertIndex > dropInfo.DragInfo.SourceIndex)
            {
                Rule.MoveCriteria(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
                Criteria.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
            }
            else
            {
                Rule.MoveCriteria(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
                Criteria.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
            }
            IsDirty = true;
        }
    }

    public void AddCriteria(LootCriteria newCriteria)
    {
        Rule.AddCriteria(newCriteria);

        var vm = LootCriteriaViewModelFactory.CreateViewModel(newCriteria);
        vm.PropertyChanged += Vm_PropertyChanged;
        Criteria.Add(vm);

        IsDirty = true;
        SelectedCriteria = vm;
    }
}
