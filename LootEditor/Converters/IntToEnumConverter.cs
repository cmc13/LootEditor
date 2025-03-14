﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace LootEditor.Converters;

public class IntToEnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter != null && parameter is Type t)
            return Enum.ToObject(t, value);
        if (targetType.IsEnum)
            return Enum.ToObject(targetType, value);
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return System.Convert.ChangeType(value, targetType);
    }
}
