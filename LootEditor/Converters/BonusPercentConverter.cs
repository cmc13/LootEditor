using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters;

internal class BonusPercentConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double percent && (targetType == typeof(double) || (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(targetType) == typeof(double))))
        {
            return percent - 1.0;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double percent && (targetType == typeof(double) || (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(targetType) == typeof(double))))
        {
            return percent + 1.0;
        }

        return value;
    }
}
