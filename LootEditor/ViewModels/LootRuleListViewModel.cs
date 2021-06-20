﻿using GongSolutions.Wpf.DragDrop;
using LootEditor.Dialogs;
using LootEditor.Models;
using LootEditor.Models.Enums;
using LootEditor.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LootEditor.ViewModels
{
    public class LootRuleListViewModel : ObservableRecipient, IDropTarget
    {
        private readonly LootFile lootFile;
        private bool isDirty = false;
        private LootRuleViewModel selectedRule = null;
        private string filter;
        private readonly DialogService dialogService = new();
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

            ToggleDisabledCommand = new RelayCommand(() => SelectedRule.ToggleDisabledCommand.Execute(null), () => SelectedRule != null);
            AddRuleCommand = new(AddRule);
            CloneRuleCommand = new(CloneRule, SelectedRule_CanExecute);
            DeleteRuleCommand = new(DeleteRule, SelectedRule_CanExecute);
            MoveSelectedItemDownCommand = new(MoveSelectedItemDown, SelectedRule_CanExecute);
            MoveSelectedItemUpCommand = new(MoveSelectedItemUp, SelectedRule_CanExecute);
            CutItemCommand = new(CutRule, SelectedRule_CanExecute);
            CopyItemCommand = new(CopyRule, SelectedRule_CanExecute);
            PasteItemCommand = new(PasteRule, () => Clipboard.ContainsData(typeof(LootRule).Name));
            AddRuleFromTemplateCommand = new(AddRuleFromTemplate, s => s != null);
            SaveRuleAsTemplateCommand = new(SaveRuleAsTemplate, SelectedRule_CanExecute);

            templateService.TemplatesChanged += (s, e) => OnPropertyChanged(nameof(RuleTemplates));
        }

        public ObservableCollection<LootRuleViewModel> LootRules { get; } = new ObservableCollection<LootRuleViewModel>();

        public IEnumerable<MenuItemViewModel> RuleTemplates
        {
            get
            {
                var templates = templateService.GetTemplates();
                return templates.Select(t => new MenuItemViewModel(t.Name, async () => await AddRuleFromTemplate(t).ConfigureAwait(false)));
            }
        }

        public LootRuleViewModel SelectedRule
        {
            get => selectedRule;
            set
            {
                if (selectedRule != value)
                {
                    selectedRule = value;
                    OnPropertyChanged(nameof(SelectedRule));

                    CutItemCommand?.NotifyCanExecuteChanged();
                    CopyItemCommand.NotifyCanExecuteChanged();
                    MoveSelectedItemUpCommand?.NotifyCanExecuteChanged();
                    MoveSelectedItemDownCommand?.NotifyCanExecuteChanged();
                    ToggleDisabledCommand?.NotifyCanExecuteChanged();
                    CloneRuleCommand?.NotifyCanExecuteChanged();
                    DeleteRuleCommand?.NotifyCanExecuteChanged();
                    SaveRuleAsTemplateCommand?.NotifyCanExecuteChanged();
                }
            }
        }

        public string Filter
        {
            get => filter;
            set
            {
                if (filter != value)
                {
                    filter = value;
                    OnPropertyChanged(nameof(Filter));
                }
            }
        }

        public bool IsDirty
        {
            get => isDirty || LootRules.Any(r => r.IsDirty);
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public RelayCommand AddRuleCommand { get; }
        public RelayCommand CloneRuleCommand { get; }
        public RelayCommand DeleteRuleCommand { get; }
        public RelayCommand<int> MoveSelectedItemDownCommand { get; }
        public RelayCommand<int> MoveSelectedItemUpCommand { get; }
        public RelayCommand CutItemCommand { get; }
        public RelayCommand CopyItemCommand { get; }
        public RelayCommand PasteItemCommand { get; }
        public RelayCommand ToggleDisabledCommand { get; }
        public AsyncRelayCommand<RuleTemplate> AddRuleFromTemplateCommand { get; }
        public AsyncRelayCommand SaveRuleAsTemplateCommand { get; }

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

        private void CutRule()
        {
            CopyRule();
            DeleteRule();
        }

        private void CopyRule()
        {
            Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
            PasteItemCommand?.NotifyCanExecuteChanged();
        }

        private void PasteRule()
        {
            var newRule = (LootRule)Clipboard.GetData(typeof(LootRule).Name);
            AddRule(newRule);
        }

        private void MoveSelectedItemUp(int index)
        {
            LootRules.Move(index, index - 1);
            lootFile.MoveRule(index, index - 1);
            IsDirty = true;
        }

        private void MoveSelectedItemDown(int index)
        {
            LootRules.Move(index, index + 1);
            lootFile.MoveRule(index, index + 1);
            IsDirty = true;
        }

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

        private void AddRule()
        {
            var rule = new LootRule()
            {
                Name = "New Rule",
                Action = LootAction.Keep
            };

            AddRule(rule);
        }

        public void ReplaceRule(LootRule rule)
        {
            var matchingRule = LootRules.FirstOrDefault(r => r.Name.Equals(rule.Name));
            matchingRule.PropertyChanged -= Vm_PropertyChanged;
            LootRules.Remove(matchingRule);
            lootFile.RemoveRule(matchingRule.Rule);

            AddRule(rule);
        }

        private bool SelectedRule_CanExecute<T>(T _) => SelectedRule_CanExecute();

        private bool SelectedRule_CanExecute()
        {
            return SelectedRule != null;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LootRuleViewModel.IsDirty))
            {
                OnPropertyChanged(nameof(IsDirty));
            }
        }

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

        public async Task SaveRuleAsTemplate()
        {
            await templateService.SaveRuleAsTemplate(SelectedRule.Rule).ConfigureAwait(false);
        }
    }
}
