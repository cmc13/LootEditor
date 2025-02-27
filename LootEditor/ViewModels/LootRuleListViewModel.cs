﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Models;
using LootEditor.Models.Enums;
using LootEditor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.ViewModels;

public partial class LootRuleListViewModel : DirtyViewModel, IDropTarget
{
    private readonly LootFile lootFile;
    private readonly TemplateService templateService = new();

    public LootRuleListViewModel(LootFile lootFile)
    {
        this.lootFile = lootFile;

        foreach (var rule in lootFile.Rules)
        {
            var vm = new LootRuleViewModel(rule);
            vm.PropertyChanged += Vm_PropertyChanged;
            LootRules.Add(vm);
        }

        templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(RuleTemplates));
    }

    public string Name { get; } = "Rules";

    public ObservableCollection<LootRuleViewModel> LootRules { get; } = [];

    public IEnumerable<MenuItemViewModel> RuleTemplates
    {
        get
        {
            var templates = templateService.GetTemplates();
            return templates.Select(t => new MenuItemViewModel(t.Name, async () => await AddRuleFromTemplate(t).ConfigureAwait(false)));
        }
    }

    [ObservableProperty]
    private LootRuleViewModel selectedRule = null;

    partial void OnSelectedRuleChanged(LootRuleViewModel value)
    {
        CutRuleCommand?.NotifyCanExecuteChanged();
        CopyRuleCommand?.NotifyCanExecuteChanged();
        CloneRuleCommand?.NotifyCanExecuteChanged();
        DeleteRuleCommand?.NotifyCanExecuteChanged();
        MoveSelectedItemUpCommand?.NotifyCanExecuteChanged();
        MoveSelectedItemDownCommand?.NotifyCanExecuteChanged();
        ToggleDisabledCommand?.NotifyCanExecuteChanged();
        SaveRuleAsTemplateCommand?.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private string filter;

    public override bool IsDirty
    {
        get => base.IsDirty || LootRules.Any(r => r.IsDirty);
        set => base.IsDirty = value;
    }

    public void AddRule(LootRule rule)
    {
        lootFile.AddRule(rule);

        var vm = new LootRuleViewModel(rule);
        vm.PropertyChanged += Vm_PropertyChanged;
        LootRules.Add(vm);

        SelectedRule = vm;
        IsDirty = true;
    }

    public void Clean()
    {
        foreach (var rule in LootRules.Where(r => r.IsDirty))
            rule.Clean();
        IsDirty = false;
    }

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is LootRuleViewModel)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.InsertIndex != dropInfo.DragInfo.SourceIndex)
        {
            if (dropInfo.InsertIndex > dropInfo.DragInfo.SourceIndex)
            {
                lootFile.MoveRule(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
                LootRules.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex - 1);
            }
            else
            {
                lootFile.MoveRule(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
                LootRules.Move(dropInfo.DragInfo.SourceIndex, dropInfo.InsertIndex);
            }
            IsDirty = true;
        }
    }

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    private void CutRule()
    {
        CopyRule();
        DeleteRule();
    }

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    private void CopyRule()
    {
        Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
        PasteRuleCommand?.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PasteRule_CanExecute))]
    private void PasteRule()
    {
        var newRule = (LootRule)Clipboard.GetData(typeof(LootRule).Name);
        AddRule(newRule);
    }

    [RelayCommand(CanExecute = nameof(MoveItem_CanExecute))]
    private void MoveSelectedItemUp(int index)
    {
        LootRules.Move(index, index - 1);
        lootFile.MoveRule(index, index - 1);
        IsDirty = true;
    }

    [RelayCommand(CanExecute = nameof(MoveItem_CanExecute))]
    private void MoveSelectedItemDown(int index)
    {
        LootRules.Move(index, index + 1);
        lootFile.MoveRule(index, index + 1);
        IsDirty = true;
    }

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    private void DeleteRule()
    {
        var sel = SelectedRule;
        if (sel != null)
        {
            sel.PropertyChanged -= Vm_PropertyChanged;
            LootRules.Remove(sel);
            lootFile.RemoveRule(sel.Rule);
            IsDirty = true;
        }
    }

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    private void CloneRule()
    {
        var sel = SelectedRule;
        if (sel != null)
        {
            var newRule = sel.CloneRule();
            lootFile.AddRule(newRule);

            var vm = new LootRuleViewModel(newRule);
            vm.PropertyChanged += Vm_PropertyChanged;
            LootRules.Add(vm);

            IsDirty = true;
            SelectedRule = vm;
        }
    }

    [RelayCommand]
    private void AddRule()
    {
        var rule = new LootRule()
        {
            Name = "New Rule",
            Action = LootAction.Keep
        };

        AddRule(rule);
    }

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    private void ToggleDisabled() => SelectedRule.ToggleDisabledCommand.Execute(null);

    [RelayCommand(CanExecute = nameof(SelectedRule_CanExecute))]
    public async Task SaveRuleAsTemplate()
    {
        await templateService.SaveRuleAsTemplate(SelectedRule.Rule).ConfigureAwait(false);
    }

    [RelayCommand(CanExecute = nameof(AddRuleFromTemplate_CanExecute))]
    public async Task AddRuleFromTemplate(RuleTemplate template)
    {
        try
        {
            var rule = await templateService.GetRuleFromTemplate(template).ConfigureAwait(false);
            AddRule(rule);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add rule from template {template.Name}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void ReplaceRule(LootRule rule)
    {
        var matchingRule = LootRules.FirstOrDefault(r => r.Name.Equals(rule.Name));
        matchingRule.PropertyChanged -= Vm_PropertyChanged;
        LootRules.Remove(matchingRule);
        lootFile.RemoveRule(matchingRule.Rule);

        AddRule(rule);
    }

    private bool MoveItem_CanExecute(int _) => SelectedRule_CanExecute();

    private bool SelectedRule_CanExecute() => SelectedRule != null;

    private static bool AddRuleFromTemplate_CanExecute(RuleTemplate template) => template != null;

    private static bool PasteRule_CanExecute() => Clipboard.ContainsData(typeof(LootRule).Name);

    private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LootRuleViewModel.IsDirty))
        {
            OnPropertyChanged(nameof(IsDirty));
        }
    }
}
