using System;
using System.ComponentModel;

namespace LootEditor.ViewModels;

public class AcceptPendingChangeEventArgs : CancelEventArgs
{
    public AcceptPendingChangeEventArgs(string propertyName, object oldValue, object newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public object OldValue { get; }
    public object NewValue { get; }
    public string PropertyName { get; }
}

public interface IAcceptPendingChange
{
    event EventHandler<AcceptPendingChangeEventArgs> AcceptPendingChange;
}
