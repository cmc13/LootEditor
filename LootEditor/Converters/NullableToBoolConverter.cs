using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters;

public class NullableToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(string))
            return ((bool)value) ? string.Empty : null;

        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            if ((bool)value)
            {
                return Activator.CreateInstance(underlyingType);
            }
            else
                return null;
        }

        return null;
    }
}
