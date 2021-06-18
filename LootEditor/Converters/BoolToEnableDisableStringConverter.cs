using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class BoolToEnableDisableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDisabled && isDisabled)
            {
                return "Enable Rule";
            }

            return "Disable Rule";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
