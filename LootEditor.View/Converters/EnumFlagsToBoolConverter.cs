using LootEditor.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            return checkedSlots;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int combined = 0;
            //var list = ((string)value).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var slotValue in list.Select(s => (ArmorSlot)Enum.Parse(typeof(ArmorSlot), s)))
            //    combined |= (int)slotValue;
            return combined;
        }
    }
}
