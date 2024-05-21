using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.Converters;

public class NameToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var name = value as string;

        var count = 0;
        foreach (var ch in name.TrimStart(' '))
        {
            if (ch != '*') break;
            count++;
        }

        return count switch
        {
            0 => FontWeights.Thin,
            1 => FontWeights.Normal,
            2 => FontWeights.Bold,
            3 => FontWeights.ExtraBold,
            _ => (object)FontWeights.ExtraBlack,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
