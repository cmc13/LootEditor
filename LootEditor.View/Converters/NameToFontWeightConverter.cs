using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
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

            switch (count)
            {
                case 0: return FontWeights.Thin;
                case 1: return FontWeights.Normal;
                case 2: return FontWeights.Bold;
                case 3: return FontWeights.ExtraBold;
                case 4:
                default: return FontWeights.ExtraBlack;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
