using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value.GetType().IsEnum)
            return System.Convert.ToInt32(value);
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
