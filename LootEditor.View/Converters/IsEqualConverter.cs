using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class IsEqualConverter : IValueConverter
    {
        public bool Reverse { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isEqual = parameter.Equals(value);
            return Reverse ? !isEqual : isEqual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b)
                {

                    return Reverse ? GetDefaultValue(targetType) : parameter;
                }
                else
                {
                    return Reverse ? parameter : GetDefaultValue(targetType);
                }
            }

            return value;
        }

        private static object GetDefaultValue(Type targetType)
        {
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
                return Activator.CreateInstance(targetType);
            else
                return null;
        }
    }
}
