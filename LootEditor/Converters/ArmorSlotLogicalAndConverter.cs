using LootEditor.Models.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class ArmorSlotLogicalAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var slotValue = (EquippableSlot)values[0];
            var testValue = (EquippableSlot)values[1];
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
