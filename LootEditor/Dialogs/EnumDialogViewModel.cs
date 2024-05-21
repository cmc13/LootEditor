using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LootEditor.Dialogs;

public class EnumDialogViewModel<TEnum> : ObservableRecipient where TEnum : struct
{
    private bool doForAllItems = false;

    public string Caption { get; }
    public string Message { get; }
    public List<TEnum> EnumValues { get; }
    public TEnum DialogResult { get; private set; }
    public Action CloseDialog { get; set; } = null;

    public RelayCommand<TEnum> CloseCommand { get; }

    public EnumDialogViewModel(string message, string caption)
    {
        Message = message;
        Caption = caption;
        EnumValues = new List<TEnum>(Enum.GetValues(typeof(TEnum)).Cast<TEnum>());

        CloseCommand = new RelayCommand<TEnum>(r =>
        {
            DialogResult = r;
            CloseDialog();
        });
    }

    public bool DoForAllItems
    {
        get => doForAllItems;
        set
        {
            if (doForAllItems != value)
            {
                doForAllItems = value;
                OnPropertyChanged(nameof(DoForAllItems));
            }
        }
    }
}
