using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using LootEditor.Model;
using LootEditor.Model.Enums;
using LootEditor.View.ViewModel;

namespace LootEditor.View.Dialogs
{
    public class BulkUpdateViewModel : ViewModelBase
    {
        private LootCriteriaViewModel lootCriteriaViewModel = null;
        private string name = null;
        private LootAction? action;
        private bool? applyToDisabled;

        public LootCriteriaViewModel LootCriteriaViewModel
        {
            get => lootCriteriaViewModel;
            set
            {
                if (lootCriteriaViewModel != value)
                {
                    if (lootCriteriaViewModel != null)
                        lootCriteriaViewModel.PropertyChanged -= LootCriteriaViewModel_PropertyChanged;

                    lootCriteriaViewModel = value;
                    RaisePropertyChanged(nameof(LootCriteriaViewModel));

                    if (lootCriteriaViewModel != null)
                        lootCriteriaViewModel.PropertyChanged += LootCriteriaViewModel_PropertyChanged;
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public LootAction? Action
        {
            get => action;
            set
            {
                if (action != value)
                {
                    action = value;
                    RaisePropertyChanged(nameof(Action));
                }
            }
        }

        public bool? ApplyToDisabled
        {
            get => applyToDisabled;
            set
            {
                if (applyToDisabled != value)
                {
                    applyToDisabled = value;
                    RaisePropertyChanged(nameof(ApplyToDisabled));
                }
            }
        }

        public BulkUpdateViewModel()
        {
            var crit = LootCriteria.CreateLootCriteria(default);
            LootCriteriaViewModel = LootCriteriaViewModelFactory.CreateViewModel(crit);
        }

        private void LootCriteriaViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                var crit = LootCriteria.CreateLootCriteria(LootCriteriaViewModel.Type);

                // Need to run this on the UI thread
                DispatcherHelper.RunAsync(() => LootCriteriaViewModel = LootCriteriaViewModelFactory.CreateViewModel(crit));
            }
        }
    }
}
