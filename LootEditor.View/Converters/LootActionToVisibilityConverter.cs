using LootEditor.Model;
using LootEditor.Model.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class LootActionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((LootAction)value == LootAction.KeepUpTo)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
