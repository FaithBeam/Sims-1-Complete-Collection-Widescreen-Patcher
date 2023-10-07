using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.UI.Converters;

public class ResolutionColorCodingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Resolution r)
        {
            return null;
        }

        if (r.Width % 1600 == 0)
        {
            if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
            {
                return Brush.Parse("#f66");
            }

            if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
            {
                return Brush.Parse("#ff1e1e");
            }
        }
        
        if (r.AspectRatio is { Numerator: 4, Denominator: 3 })
        {
            if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
            {
                return Brush.Parse("#97ef7f");
            }

            if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
            {
                return Brush.Parse("#165607");
            }
        }

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}