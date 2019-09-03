using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LootEditor.Model;

namespace LootEditor.View.ViewModel
{
    public class LootRuleViewModel : ViewModelBase
    {
        private readonly LootRule rule;
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
            get => rule.Name;
            set
            {
                if (rule.Name != value)
                {
                    rule.Name = value;
                    RaisePropertyChanged(nameof(Name));
                    IsDirty = true;
                }
            }
        }

        public LootAction Action
        {
            get => rule.Action;
            set
            {
                if (rule.Action != value)
                {
                    rule.Action = value;
                    RaisePropertyChanged(nameof(Action));
                    IsDirty = true;
                }
            }
        }

        public int KeepUpToCount
        {
            get => rule.KeepUpToCount;
            set
            {
                if (rule.KeepUpToCount != value)
                {
                    rule.KeepUpToCount = value;
                    RaisePropertyChanged(nameof(KeepUpToCount));
                    IsDirty = true;
                }
            }
        }

        public bool IsDisabled => rule.Criteria.Any(c => c.Type == LootCriteriaType.DisabledRule && ((ValueLootCriteria<bool>)c).Value == true);

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
            this.rule = rule;
            Criteria.Clear();
            foreach (var crit in rule.Criteria)
                Criteria.Add(new LootCriteriaViewModel(crit));

            AddCriteriaCommand = new RelayCommand(() => { });

            CloneCriteriaCommand = new RelayCommand(() =>
            {

            }, () => SelectedCriteria != null);

            DeleteCriteriaCommand = new RelayCommand(() =>
            {

            }, () => SelectedCriteria != null);
        }

        public void Clean()
        {
            IsDirty = false;
        }
    }
}
