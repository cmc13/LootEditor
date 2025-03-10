﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.Converters;

public class ReverseBooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Convert bool or Nullable&lt;bool&gt; to Visibility
    /// </summary>
    /// <param name="value">bool or Nullable&lt;bool&gt;</param>
    /// <param name="targetType">Visibility</param>
    /// <param name="parameter">null</param>
    /// <param name="culture">null</param>
    /// <returns>Visible or Collapsed</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool bValue = false;
        if (value is bool v)
        {
            bValue = v;
        }
        else if (value is bool?)
        {
            bool? tmp = (bool?)value;
            bValue = tmp ?? false;
        }
        return (bValue) ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Convert Visibility to boolean
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == Visibility.Collapsed;
        }
        else
        {
            return false;
        }
    }
}
