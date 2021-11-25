using System.IO;
using Serilog;
using Sims1WidescreenPatcher.Far;
using Sims1WidescreenPatcher.Wrappers;

namespace Sims1WidescreenPatcher.Uninstall
{
    public static class UninstallPatch
    {
        public static void DoUninstall(string path)
        {
            Log.Debug("Begin uninstall");
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
            var directory = Path.GetDirectoryName(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var backupPath = Path.Combine(directory, filenameWithoutExtension);
            File.Move($@"{backupPath} Backup.exe", path);
            GraphicsWrapper.TryRemoveWrapper(path);
            Images.RemoveGraphics(path);
            Log.Debug("Finished uninstall");
        }
    }
}
