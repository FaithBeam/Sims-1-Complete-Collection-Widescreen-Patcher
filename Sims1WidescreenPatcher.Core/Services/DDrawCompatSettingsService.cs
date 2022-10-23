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
    }
}