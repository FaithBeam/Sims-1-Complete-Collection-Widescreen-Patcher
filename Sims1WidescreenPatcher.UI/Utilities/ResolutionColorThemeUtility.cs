using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.UI.Utilities;

public static class ResolutionColorThemeUtility
{
    private static readonly Dictionary<string, IBrush> DarkTheme = new()
    {
        {"4:3", Brush.Parse("#404556")},
        {"16:9", Brush.Parse("#60515c")},
        {"16:10", Brush.Parse("#777076")},
        {"unknown", Brush.Parse("#597d7c")},
    };
    private static readonly Dictionary<string, IBrush> LightTheme = new()
    {
        {"4:3", Brush.Parse("#00ff00")},
        {"16:9", Brush.Parse("#00ffff")},
        {"16:10", Brush.Parse("#0000ff")},
        {"unknown", Brush.Parse("#ff0000")},
    };
    
    public static IBrush GetColor(AspectRatio ratio)
    {
        var theme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Default;
        if (theme == ThemeVariant.Light)
        {
            return LightTheme.TryGetValue(ratio.ToString(), out var value) ? value : LightTheme["unknown"];
        }
        if (theme == ThemeVariant.Dark)
        {
            return DarkTheme.TryGetValue(ratio.ToString(), out var value) ? value : DarkTheme["unknown"];
        }

        throw new Exception("Unknown theme");
    }
}