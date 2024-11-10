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
    private static readonly IBrush BackgroundGoodLight = Brush.Parse("#97ef7f");
    private static readonly IBrush BackgroundGoodDark = Brush.Parse("#165607");
    private static readonly IBrush BackgroundBadLight = Brush.Parse("#f66");
    private static readonly IBrush BackgroundBadDark = Brush.Parse("#be1e1e");

    private static readonly IBrush PointeroverGoodLight = Brush.Parse("#4ca43a");
    private static readonly IBrush PointeroverGoodDark = Brush.Parse("#4c893a");
    private static readonly IBrush PointeroverBadLight = Brush.Parse("#cd3740");
    private static readonly IBrush PointeroverBadDark = Brush.Parse("#fd5e4b");

    private static readonly IBrush SelectedGoodLight = Brush.Parse("#7bd265");
    private static readonly IBrush SelectedGoodDark = Brush.Parse("#326f22");
    private static readonly IBrush SelectedBadLight = Brush.Parse("#e0494e");
    private static readonly IBrush SelectedBadDark = Brush.Parse("#dd4034");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Resolution r)
        {
            return null;
        }

        if (parameter is not string s)
        {
            return null;
        }

        switch (s)
        {
            case "Background":
            {
                if (r.Width == 1600)
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return BackgroundBadLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return BackgroundBadDark;
                    }
                }

                if (r.AspectRatio is { Numerator: 4, Denominator: 3 })
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return BackgroundGoodLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return BackgroundGoodDark;
                    }
                }

                break;
            }
            case "Pointerover":
            {
                if (r.Width == 1600)
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return PointeroverBadLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return PointeroverBadDark;
                    }
                }

                if (r.AspectRatio is { Numerator: 4, Denominator: 3 })
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return PointeroverGoodLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return PointeroverGoodDark;
                    }
                }

                if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                {
                    return Brush.Parse("#19000000");
                }

                if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                {
                    return Brush.Parse("#19FFFFFF");
                }

                break;
            }
            case "Selected":
            {
                if (r.Width == 1600)
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return SelectedBadLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return SelectedBadDark;
                    }
                }

                if (r.AspectRatio is { Numerator: 4, Denominator: 3 })
                {
                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                    {
                        return SelectedGoodLight;
                    }

                    if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                    {
                        return SelectedGoodDark;
                    }
                }

                if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                {
                    return Brush.Parse("#29000000");
                }

                if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                {
                    return Brush.Parse("#09FFFFFF");
                }

                break;
            }
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotImplementedException();
    }
}
