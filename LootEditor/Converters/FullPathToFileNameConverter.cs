using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class FullPathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string str)
                    return Path.GetFileNameWithoutExtension(str);
            }
            catch
            { }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
