using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class HexStringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int iValue)
            {
                return iValue.ToString("X");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                if (strValue.StartsWith("0x"))
                    strValue = strValue.Substring(2);

                try
                {
                    return System.Convert.ToInt32(strValue, 16);
                }
                catch
                {
                    return 0;
                }
            }

            return value;
        }
    }
}
