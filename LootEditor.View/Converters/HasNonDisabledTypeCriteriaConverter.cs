using LootEditor.View.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class HasNonDisabledTypeCriteriaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is IEnumerable<LootCriteriaViewModel> list && list.Any(vm => vm.Type != Model.Enums.LootCriteriaType.DisabledRule) ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
