using PatternFinder;
using Serilog;

namespace Sims1WidescreenPatcher.Utilities;

public static class PatchUtility
{
    private const string BytePattern = "20 03 ?? ?? ?? ?? ?? 58 02";

    public static bool IsValidSims(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return false;
        Log.Information("Begin check Sims executable is valid");
        var pattern = Pattern.Transform(BytePattern);
        var bytes = File.ReadAllBytes(path);
        var patternFound = Pattern.Find(bytes, pattern, out _);
        Log.Debug("{@Path} is valid: {PatternFound}", path, patternFound);
        return patternFound;
    }

    public static void Patch(string path, int width, int height)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

        Log.Information("Begin patching of Sims executable");
        Log.Debug("Path {@Path}", path);
        Log.Debug("Width {@Width}", width);
        Log.Debug("Height {@Height}", height);
        var pattern = Pattern.Transform(BytePattern);
        var bytes = File.ReadAllBytes(path);

        if (!Pattern.Find(bytes, pattern, out var foundOffset))
        {
            Log.Debug("Pattern {@Pattern} not found in {@Path}", BytePattern, path);
            return;
        }

        BackupSims(path);

        var widthBytes = BitConverter.GetBytes(width);
        bytes[foundOffset] = widthBytes[0];
        bytes[foundOffset + 1] = widthBytes[1];

        var heightBytes = BitConverter.GetBytes(height);
        bytes[foundOffset + 2 + 5] = heightBytes[0];
        bytes[foundOffset + 2 + 5 + 1] = heightBytes[1];

        File.SetAttributes(path, FileAttributes.Normal);
        File.WriteAllBytes(path, bytes);
        Log.Debug("Patched {@Path} at offset {@Offset}", path, foundOffset);
        Log.Information("End patching of Sims executable");
    }

    public static bool SimsBackupExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        Log.Information("Begin check backup exists for Sims executable");
        Log.Debug("Path {Path}", path);
        var fileName = Path.GetFileNameWithoutExtension(path);
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var backupPath = Path.Combine(dir, fileName) + " Backup.exe";
        var backupExists = File.Exists(backupPath);
        Log.Debug("Backup for {@Path} exists at {@BackupPath}: {@BackupExists}", path, backupPath, backupExists);
        Log.Information("End check backup exists for Sims executable");
        return backupExists;
    }

    private static void BackupSims(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        Log.Information("Begin backup of Sims executable");
        var fileName = Path.GetFileNameWithoutExtension(path);
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var backupPath = Path.Combine(dir, fileName) + " Backup.exe";
        if (File.Exists(backupPath))
        {
            Log.Information("Backup already exists at @{BackupPath}", backupPath);
            return;
        }
        File.Copy(path, backupPath);
        File.SetAttributes(backupPath, FileAttributes.Normal);
        Log.Debug("{@Path} backed up to {@BackupPath}", path, backupPath);
        Log.Information("End backup of Sims executable");
    }
}