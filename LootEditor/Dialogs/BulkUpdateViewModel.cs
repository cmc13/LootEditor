using LootEditor.Models;
using LootEditor.Models.Enums;
using LootEditor.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace LootEditor.Dialogs;

public class BulkUpdateViewModel : ObservableRecipient
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
                OnPropertyChanged(nameof(LootCriteriaViewModel));

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
                OnPropertyChanged(nameof(Name));
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
                OnPropertyChanged(nameof(Action));
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
                OnPropertyChanged(nameof(ApplyToDisabled));
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
        if (e.PropertyName == nameof(LootCriteriaViewModel.Type))
        {
            var crit = LootCriteria.CreateLootCriteria(LootCriteriaViewModel.Type);

            // Need to run this on the UI thread
            Application.Current.Dispatcher.Invoke(() => LootCriteriaViewModel = LootCriteriaViewModelFactory.CreateViewModel(crit));
        }
    }
}
