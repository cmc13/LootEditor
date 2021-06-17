using LootEditor.Dialogs;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class NullableBoolToWhichRulesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b ? WhichRules.Disabled : WhichRules.Enabled;
            }

            return WhichRules.All;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WhichRules r)
            {
                switch (r)
                {
                    case WhichRules.All: return null;
                    case WhichRules.Enabled: return false;
                    case WhichRules.Disabled: return true;
                }
            }

            return value;
        }
    }
}
