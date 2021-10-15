using System;
using System.Globalization;
using System.Windows.Data;
using Sims1WidescreenPatcher.Resolutions;

namespace Sims1WidescreenPatcher.UI.WPF.Converters
{
    [ValueConversion(typeof(Resolution), typeof(string))]
    public class ResolutionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Resolution) value)?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var split = ((string) value)?.Split('x');
            var resolution = new Resolution()
            {
                Width = System.Convert.ToInt32(split?[0]),
                Height = System.Convert.ToInt32(split?[1])
            };
            return resolution;
        }
    }
}