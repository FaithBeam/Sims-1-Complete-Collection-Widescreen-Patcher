using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Sims1WidescreenPatcher.UI.WPF.ValidationRules
{
    public class ResolutionValidationRule : ValidationRule
    {
        private readonly Regex _rx = new Regex(@"^\d+x\d+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return new ValidationResult(_rx.IsMatch(value as string ?? string.Empty), "Invalid resolution.");
        }
    }
}