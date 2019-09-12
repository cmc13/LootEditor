using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType().IsEnum)
            {
                var td = TypeDescriptor.GetConverter(value.GetType());
                if (td != null)
                {
                    return td.ConvertToInvariantString(value);
                }
                else
                {
                    var fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        if (attributes.Length > 0)
                            return !string.IsNullOrEmpty(attributes[0].Description) ? attributes[0].Description : value.ToString();
                    }
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
