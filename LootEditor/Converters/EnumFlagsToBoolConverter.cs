using LootEditor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LootEditor.Converters
{
    public class EnumFlagsToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var slots = (EquippableSlot)value;
            var checkedSlots = new List<EquippableSlot>();
            foreach (EquippableSlot enumValue in Enum.GetValues(typeof(EquippableSlot)))
            {
                if ((slots & enumValue) != 0)
                {
                    checkedSlots.Add(enumValue);
                }
            }

            return string.Join(",", checkedSlots);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int combined = 0;
            var items = ((string)value).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => (EquippableSlot)Enum.Parse(typeof(EquippableSlot), s));
            foreach (EquippableSlot slotValue in items)
                combined |= (int)slotValue;
            return combined;
        }
    }
}
