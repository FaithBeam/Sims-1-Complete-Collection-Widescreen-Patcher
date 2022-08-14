using Serilog;

namespace Sims1WidescreenPatcher.Utilities;

public static class UninstallUtility
{
    public static void Uninstall(string path)
    {
        Log.Information("Begin uninstall");
        Log.Debug("Path {@Path}", path);
        File.SetAttributes(path, FileAttributes.Normal);
        Log.Debug("Delete {@Path}", path);
        File.Delete(path);
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
        var backupPath = Path.Combine(dir, fileNameWithoutExtension) + " Backup.exe";
        Log.Debug("Move {@BackupPath} to {@Destination}", backupPath, path);
        File.Move(backupPath, path);
        WrapperUtility.RemoveWrapper(path);
        Images.Images.RemoveGraphics(path);
        Log.Information("Finish uninstall");
    }
}