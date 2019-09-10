using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Model;
using LootEditor.Model.Enums;
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

            AddRuleCommand = new RelayCommand(AddRule);

            CloneRuleCommand = new RelayCommand(CloneRule, SelectedRule_CanExecute);

            DeleteRuleCommand = new RelayCommand(DeleteRule, SelectedRule_CanExecute);

            MoveSelectedItemDownCommand = new RelayCommand<int>(index =>
            {
                LootRules.Move(index, index + 1);
                lootFile.MoveRule(index, index + 1);
                IsDirty = true;
            }, _ => SelectedRule_CanExecute());

            MoveSelectedItemUpCommand = new RelayCommand<int>(index =>
            {
                LootRules.Move(index, index - 1);
                lootFile.MoveRule(index, index - 1);
                IsDirty = true;
            }, _ => SelectedRule_CanExecute());

            CutItemCommand = new RelayCommand(() =>
            {
                Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
                DeleteRuleCommand.Execute(null);
                PasteItemCommand?.RaiseCanExecuteChanged();
            }, SelectedRule_CanExecute);

            CopyItemCommand = new RelayCommand(() =>
            {
                Clipboard.SetData(typeof(LootRule).Name, SelectedRule.Rule);
                PasteItemCommand?.RaiseCanExecuteChanged();
            }, SelectedRule_CanExecute);

            PasteItemCommand = new RelayCommand(() =>
            {
                var newRule = (LootRule)Clipboard.GetData(typeof(LootRule).Name);

                lootFile.AddRule(newRule);

                var vm = new LootRuleViewModel(newRule);
                LootRules.Add(vm);

                IsDirty = true;
                SelectedRule = vm;
            }, () => Clipboard.ContainsData(typeof(LootRule).Name));

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

        public RelayCommand AddRuleCommand { get; }
        public RelayCommand CloneRuleCommand { get; }
        public RelayCommand DeleteRuleCommand { get; }
        public RelayCommand<int> MoveSelectedItemDownCommand { get; }
        public RelayCommand<int> MoveSelectedItemUpCommand { get; }
        public RelayCommand CutItemCommand { get; }
        public RelayCommand CopyItemCommand { get; }
        public RelayCommand PasteItemCommand { get; }
        public RelayCommand ToggleDisabledCommand { get; }

        public void Clean()
        {
            IsDirty = false;
            foreach (var rule in LootRules.Where(r => r.IsDirty))
                rule.Clean();
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

        private void DeleteRule()
        {
            var sel = SelectedRule;
            if (sel != null)
            {
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

            lootFile.AddRule(rule);

            var vm = new LootRuleViewModel(rule);
            LootRules.Add(vm);

            SelectedRule = vm;
            IsDirty = true;
        }

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
