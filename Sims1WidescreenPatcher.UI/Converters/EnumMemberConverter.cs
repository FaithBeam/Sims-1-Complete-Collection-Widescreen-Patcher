using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
        if (value is not string strVal)
        {
            return null;
        }

        var t = typeof(IffPreset);
        var fieldInfo = t.GetFields(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.GetCustomAttribute<EnumMemberAttribute>()?.Value == strVal);
        return (IffPreset)(fieldInfo.GetValue(this) ?? throw new InvalidOperationException());
    }
}
