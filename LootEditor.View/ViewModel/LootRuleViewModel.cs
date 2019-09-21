using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Model;
using LootEditor.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace LootEditor.View.ViewModel
{
    public class LootRuleViewModel : ViewModelBase, IDropTarget
    {
        private bool isDirty;
        private LootCriteriaViewModel selectedCriteria;

        public bool IsDirty
        {
            get => isDirty || Criteria.Any(c => c.IsDirty);
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    RaisePropertyChanged(nameof(IsDirty));
                }
            }
        }

        public string Name
        {
            get => Rule.Name;
            set
            {
                if (Rule.Name != value)
                {
                    Rule.Name = value;
                    RaisePropertyChanged(nameof(Name));
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
                    RaisePropertyChanged(nameof(Action));
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
                    RaisePropertyChanged(nameof(KeepUpToCount));
                    IsDirty = true;
                }
            }
        }

        public LootRule Rule { get; }

        public bool IsDisabled => Rule.Criteria.Any(c => c.Type == LootCriteriaType.DisabledRule && ((ValueLootCriteria<bool>)c).Value == true);

        public ObservableCollection<LootCriteriaViewModel> Criteria { get; } = new ObservableCollection<LootCriteriaViewModel>();

        public LootCriteriaViewModel SelectedCriteria
        {
            get => selectedCriteria;
            set
            {
                if (selectedCriteria != value)
                {
                    selectedCriteria = value;
                    RaisePropertyChanged(nameof(SelectedCriteria));

                    CloneCriteriaCommand?.RaiseCanExecuteChanged();
                    DeleteCriteriaCommand?.RaiseCanExecuteChanged();
                    CutItemCommand?.RaiseCanExecuteChanged();
                    CopyItemCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand AddCriteriaCommand { get; }
        public RelayCommand CloneCriteriaCommand { get; }
        public RelayCommand DeleteCriteriaCommand { get; }
        public RelayCommand CutItemCommand { get; }
        public RelayCommand CopyItemCommand { get; }
        public RelayCommand PasteItemCommand { get; }
        public RelayCommand ToggleDisabledCommand { get; }

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

            AddCriteriaCommand = new RelayCommand(AddCriteria);

            CloneCriteriaCommand = new RelayCommand(CloneCriteria, SelectedCriteria_CanExecute);

            DeleteCriteriaCommand = new RelayCommand(RemoveCriteria, SelectedCriteria_CanExecute);

            CutItemCommand = new RelayCommand(CutItem, SelectedCriteria_CanExecute);

            CopyItemCommand = new RelayCommand(CopyItem, SelectedCriteria_CanExecute);

            PasteItemCommand = new RelayCommand(PasteItem, CanPaste);

            ToggleDisabledCommand = new RelayCommand(ToggleDisabled);
        }

        private void Criteria_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Criteria));
        }

        private bool CanPaste() => Clipboard.ContainsData(typeof(LootCriteria).Name);

        private void PasteItem()
        {
            var data = Clipboard.GetData(typeof(LootCriteria).Name) as LootCriteria;

            var newCriteria = data.Clone() as LootCriteria;
            AddCriteria(newCriteria);
        }

        private void CopyItem()
        {
            Clipboard.SetData(typeof(LootCriteria).Name, SelectedCriteria.Criteria);
            PasteItemCommand?.RaiseCanExecuteChanged();
        }

        private void CutItem()
        {
            Clipboard.SetData(typeof(LootCriteria).Name, SelectedCriteria.Criteria);
            DeleteCriteriaCommand.Execute(null);
            PasteItemCommand?.RaiseCanExecuteChanged();
        }

        private void AddCriteria()
        {
            var newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.AnySimilarColor);
            AddCriteria(newCriteria);
        }

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
            RaisePropertyChanged(nameof(IsDisabled));
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var v = sender as LootCriteriaViewModel;
            if (e.PropertyName == "Type")
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
                    RaisePropertyChanged(nameof(IsDisabled));
            }
            else if (v.Type == LootCriteriaType.DisabledRule)
                RaisePropertyChanged(nameof(IsDisabled));
            RaisePropertyChanged(nameof(IsDirty));
        }

        private bool SelectedCriteria_CanExecute() => SelectedCriteria != null;

        private void CloneCriteria()
        {
            var sel = SelectedCriteria;
            if (sel != null)
            {
                var newCriteria = sel.Criteria.Clone() as LootCriteria;
                AddCriteria(newCriteria);
            }
        }

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
                    RaisePropertyChanged(nameof(IsDisabled));
            }
        }

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
}
