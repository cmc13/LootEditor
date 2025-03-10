﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.Converters;

public class EqualityToVisibilityConverter : IValueConverter
{
    public bool Reverse { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value.Equals(parameter))
            return Reverse ? Visibility.Collapsed : Visibility.Visible;
        return Reverse ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
