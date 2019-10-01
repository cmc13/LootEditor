using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Model;
using LootEditor.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class LootRuleListViewModel : ViewModelBase, IDropTarget
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
                    RaisePropertyChanged(nameof(SelectedRule));

                    CutItemCommand?.RaiseCanExecuteChanged();
                    CopyItemCommand.RaiseCanExecuteChanged();
                    MoveSelectedItemUpCommand?.RaiseCanExecuteChanged();
                    MoveSelectedItemDownCommand?.RaiseCanExecuteChanged();
                    ToggleDisabledCommand?.RaiseCanExecuteChanged();
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
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public RelayCommand AddRuleCommand => new RelayCommand(AddRule);
        public RelayCommand CloneRuleCommand => new RelayCommand(CloneRule, SelectedRule_CanExecute);
        public RelayCommand DeleteRuleCommand => new RelayCommand(DeleteRule, SelectedRule_CanExecute);
        public RelayCommand<int> MoveSelectedItemDownCommand => new RelayCommand<int>(MoveSelectedItemDown, SelectedRule_CanExecute);
        public RelayCommand<int> MoveSelectedItemUpCommand => new RelayCommand<int>(MoveSelectedItemUp, SelectedRule_CanExecute);
        public RelayCommand CutItemCommand => new RelayCommand(CutRule, SelectedRule_CanExecute);
        public RelayCommand CopyItemCommand => new RelayCommand(CopyRule, SelectedRule_CanExecute);
        public RelayCommand PasteItemCommand => new RelayCommand(PasteRule, () => Clipboard.ContainsData(typeof(LootRule).Name));
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
            PasteItemCommand?.RaiseCanExecuteChanged();
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
                RaisePropertyChanged(nameof(IsDirty));
            }
        }
    }
}
