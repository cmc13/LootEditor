using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GongSolutions.Wpf.DragDrop;
using LootEditor.Model;

namespace LootEditor.View.ViewModel
{
    public class LootRuleViewModel : ViewModelBase, IDropTarget
    {
        private bool isDirty;
        private LootCriteriaViewModel selectedCriteria;

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
                }
            }
        }

        public RelayCommand AddCriteriaCommand { get; }
        public RelayCommand CloneCriteriaCommand { get; }
        public RelayCommand DeleteCriteriaCommand { get; }

        public LootRuleViewModel(LootRule rule)
        {
            this.Rule = rule;
            Criteria.Clear();
            foreach (var crit in rule.Criteria)
                Criteria.Add(new LootCriteriaViewModel(crit));

            AddCriteriaCommand = new RelayCommand(() =>
            {
                var newCriteria = LootCriteria.CreateLootCriteria(LootCriteriaType.AnySimilarColor);
                rule.AddCriteria(newCriteria);

                var vm = new LootCriteriaViewModel(newCriteria);
                Criteria.Add(vm);

                IsDirty = true;
                SelectedCriteria = vm;
            });

            CloneCriteriaCommand = new RelayCommand(CloneCriteria, () => SelectedCriteria != null);

            DeleteCriteriaCommand = new RelayCommand(RemoveCriteria, () => SelectedCriteria != null);
        }

        private void CloneCriteria()
        {
            var sel = SelectedCriteria;
            if (sel != null)
            {
                var newCriteria = sel.Criteria.Clone() as LootCriteria;

                Rule.AddCriteria(newCriteria);
                Criteria.Add(new LootCriteriaViewModel(newCriteria));
                IsDirty = true;
            }
        }

        private void RemoveCriteria()
        {
            var sel = SelectedCriteria;
            if (sel != null)
            {
                Rule.RemoveCriteria(sel.Criteria);
                Criteria.Remove(sel);
                IsDirty = true;
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
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
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
}
