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
                default:
                    break;
            }
        }
        await sw.WriteLineAsync("CPUAffinity=all # the default (1) caused major issues, crashes, and lag after ddrawcompat 0.4.0");
        await sw.WriteLineAsync("DisplayResolution=app # the default setting (desktop) has performance and AA issues in TS1");
        await sw.WriteLineAsync("ResolutionScale=display(1) # increases 3d rendering (Sim) resolution to your native display resolution")
        await sw.WriteLineAsync("SoftwareDevice=hal # redirect software rendering to the GPU, fixes anti-aliasing in cases where the driver cannot access CPU depth buffers"); 
        await sw.WriteLineAsync("DisplayRefreshRate=desktop # removes erroneous lock to 60fps on higher refresh rate displays when vsync is enabled");
        await sw.WriteLineAsync("AltTabFix=keepvidmem # fixes crashes/bugs when using Alt+Tab");
    }
}
