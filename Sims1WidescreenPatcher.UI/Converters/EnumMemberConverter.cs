using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Sims1WidescreenPatcher.Core.Enums;
using Sims1WidescreenPatcher.Core.Services;

namespace Sims1WidescreenPatcher.UI.Converters;

public class EnumMemberConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value switch
        {
            List<IffPreset> enums => enums.Select(EnumExtensions.GetEnumMemberValue),
            null => null,
            _ => value is not IffPreset preset ? null : EnumExtensions.GetEnumMemberValue(preset),
        };

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is null)
        {
            return null;
        }

        Enum.TryParse<IffPreset>(value as string, out var result);
        return result;
    }
}
