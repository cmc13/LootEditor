using LootEditor.Models.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.Converters;

public class LongValueKeyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LongValueKey key)
        {
            if (key == LongValueKey.Material || key == LongValueKey.Slot || key == LongValueKey.WeaponMasteryCategory ||
                key == LongValueKey.EquipSkill || key == LongValueKey.WieldReqAttribute || key == LongValueKey.ArmorSetID ||
                key == LongValueKey.Workmanship || key == LongValueKey.Unenchantable || key == LongValueKey.SlayerSpecies)
            {
                return Visibility.Collapsed;
            }
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
