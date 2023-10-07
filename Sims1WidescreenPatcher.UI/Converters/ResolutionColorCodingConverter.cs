using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.UI.Utilities;

namespace Sims1WidescreenPatcher.UI.Converters;

public class ResolutionColorCodingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not Resolution r ? null : ResolutionColorThemeUtility.GetColor(r.AspectRatio);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}