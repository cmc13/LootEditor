using LootEditor.Model.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class ArmorSlotLogicalAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var slotValue = (ArmorSlot)values[0];
            var testValue = (ArmorSlot)values[1];
            if ((slotValue & testValue) != 0)
                return true;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
