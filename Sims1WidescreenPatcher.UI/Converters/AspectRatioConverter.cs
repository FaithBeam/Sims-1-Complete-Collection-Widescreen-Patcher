using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.UI.Converters;

public class AspectRatioConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AspectRatio ratio)
        {
            return null;
        }
        return ratio.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string strVal)
        {
            return null;
        }

        if (!strVal.Contains(":"))
        {
            return null;
        }

        var split = strVal.Split(':');
        if (split.Length != 2)
        {
            return null;
        }

        if (!int.TryParse(split[0], out var width))
        {
            return null;
        }
        if (!int.TryParse(split[1], out var height))
        {
            return null;
        }
        
        return new AspectRatio(width, height);
    }
}