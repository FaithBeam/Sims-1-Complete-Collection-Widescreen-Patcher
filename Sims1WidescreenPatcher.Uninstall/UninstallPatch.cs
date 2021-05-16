using Serilog;
using Sims1WidescreenPatcher.Media;
using Sims1WidescreenPatcher.DDrawCompat;
using System.IO;

namespace Sims1WidescreenPatcher.Uninstall
{
    public static class UninstallPatch
    {
        public static void DoUninstall(string path)
        {
            Log.Debug("Begin uninstall.");
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
            var directory = Path.GetDirectoryName(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var backupPath = Path.Combine(directory, filenameWithoutExtension);
            File.Move($@"{backupPath} Backup.exe", path);
            new DllWrapper(path).TryRemoveDDrawCompat();
            new Images(path, 0, 0, null).RemoveGraphics();
            Log.Debug("Finished uninstall.");
        }
    }
}
