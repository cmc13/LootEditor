using LootEditor.Model.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace LootEditor.View.Converters
{
    public class SalvageGroupToMaterialListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SalvageGroup group)
            {
                var td = TypeDescriptor.GetConverter(typeof(Material));
                var materials = group.GetMaterials();
                return $"Materials: {string.Join(", ", materials.Select(m => td.ConvertToInvariantString(m)))}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
