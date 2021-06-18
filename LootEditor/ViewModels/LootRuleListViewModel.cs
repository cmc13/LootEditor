using GongSolutions.Wpf.DragDrop;
using LootEditor.Models;
using LootEditor.Models.Enums;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LootEditor.ViewModels
{
    public class LootRuleListViewModel : ObservableRecipient, IDropTarget
    {
        private readonly LootFile lootFile;
        private bool isDirty = false;
        private LootRuleViewModel selectedRule = null;

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
        }

        public ObservableCollection<LootRuleViewModel> LootRules { get; } = new ObservableCollection<LootRuleViewModel>();

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
            if (e.PropertyName == "IsDirty")
            {
                OnPropertyChanged(nameof(IsDirty));
            }
        }
    }
}
