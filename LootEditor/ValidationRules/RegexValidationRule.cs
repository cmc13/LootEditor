using System.Globalization;
using System.Windows.Controls;

namespace LootEditor.ValidationRules;

public class RegexValidationRule : ValidationRule
{
    public string Regex { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string s && !string.IsNullOrEmpty(s))
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(s, Regex))
            {
                return new ValidationResult(false, "Input is invalid");
            }
        }

        return ValidationResult.ValidResult;
    }
}
