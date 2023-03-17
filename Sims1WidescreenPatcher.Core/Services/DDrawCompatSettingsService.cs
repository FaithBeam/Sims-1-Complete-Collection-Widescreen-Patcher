using Sims1WidescreenPatcher.Core.Enums;

namespace Sims1WidescreenPatcher.Core.Services;

public static class DDrawCompatSettingsService
{
    public static async Task CreateDDrawCompatSettingsFile(string pathToSimsExe, params DDrawCompatEnums[] settings)
    {
        var dir = Path.GetDirectoryName(pathToSimsExe);
        if (string.IsNullOrWhiteSpace(dir))
        {
            throw new DirectoryNotFoundException($"Directory not found: {dir}");
        }
        using var sw = new StreamWriter(Path.Combine(dir, "DDrawCompat.ini"));
        foreach (var setting in settings)
        {
            switch (setting)
            {
                case DDrawCompatEnums.BorderlessFullscreen:
                    await sw.WriteLineAsync("FullscreenMode=borderless");
                    break;
                case DDrawCompatEnums.ExclusiveFullscreen:
                    await sw.WriteLineAsync("FullscreenMode=exclusive");
                    break;
                default:
                    break;
            }
        }
        await sw.WriteLineAsync("CPUAffinity=all");  // the default was changed to 1 in 0.4.0 which was the culprit for the major issues, crashes, and lag 
        await sw.WriteLineAsync("ResolutionScale=display(1)"); // enables full-res Sims on supported hardware
        await sw.WriteLineAsync("DisplayRefreshRate=desktop"); // removes erroneous lock to 60fps on higher refresh rate displays when vsync is enabled
        await sw.WriteLineAsync("AltTabFix=keepvidmem"); // fixes crashes/bugs when using Alt+Tab
        }
}