using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters;

public sealed class NameToFontSizeConverter
    : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string valueStr)
        {
            double defaultSize = 14;
            if (parameter is double od)
                defaultSize = od;
            else if (parameter is string paramString && double.TryParse(paramString, out od))
                defaultSize = od;

            var count = 0;
            foreach (var ch in valueStr.TrimStart(' '))
            {
                if (ch != '*') break;
                count++;
            }

            return count switch
            {
                0 => defaultSize,
                1 => defaultSize + 2,
                2 => defaultSize + 4,
                3 => defaultSize + 6,
                _ => defaultSize + 8,
            };
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
