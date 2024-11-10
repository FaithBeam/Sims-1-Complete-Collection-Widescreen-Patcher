namespace Sims1WidescreenPatcher.Core.Services;

public static class CheckDDrawCompatIniService
{
    public static string DDrawCompatSettingsExist(string pathToSimsExe)
    {
        var dir = Path.GetDirectoryName(pathToSimsExe);
        if (string.IsNullOrWhiteSpace(dir))
        {
            throw new ArgumentException($"Invalid directory: {pathToSimsExe}");
        }

        var ddrawSettingsPath = Path.Combine(dir, "DDrawCompat.ini");
        return File.Exists(ddrawSettingsPath) ? ddrawSettingsPath : string.Empty;
    }
}
