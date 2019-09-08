using LootEditor.Model.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class EnumFlagsToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var slots = (ArmorSlot)value;
            var checkedSlots = new List<ArmorSlot>();
            foreach (ArmorSlot enumValue in Enum.GetValues(typeof(ArmorSlot)))
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
                .Select(s => (ArmorSlot)Enum.Parse(typeof(ArmorSlot), s));
            foreach (ArmorSlot slotValue in items)
                combined |= (int)slotValue;
            return combined;
        }
    }
}
