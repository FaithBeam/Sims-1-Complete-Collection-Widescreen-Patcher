namespace Sims1WidescreenPatcher.Core.Services;

public static class RemoveDDrawCompatSettingsService
{
    public static void Remove(string pathToSettings)
    {
        File.Delete(pathToSettings);
    }
}
