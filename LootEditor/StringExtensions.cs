using System.Linq;
using System.Windows.Media;

namespace LootEditor
{
    public static class StringExtensions
    {
        public static bool IsLower(this string str)
        {
            foreach (var ch in str)
                if (char.IsUpper(ch))
                    return false;
            return true;
        }

        public static string GetColorName(this Color color)
        {
            var colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), color));
            return colorProperty != null ? colorProperty.Name : "unnamed color";
        }
    }
}
