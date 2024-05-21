using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.Converters;

public class AnyVisibleConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.Cast<Visibility>().Any(v => v == Visibility.Visible) ?
            Visibility.Visible :
            Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
