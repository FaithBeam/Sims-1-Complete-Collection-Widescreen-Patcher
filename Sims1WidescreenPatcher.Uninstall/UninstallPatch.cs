using Sims1WidescreenPatcher.Media;
using Sims1WidescreenPatcher.Models;
using Sims1WidescreenPatcher.Voodoo;
using System.IO;

namespace Sims1WidescreenPatcher.Uninstall
{
    public static class UninstallPatch
    {
        public static void DoUninstall(string path)
        {
            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
            var directory = Path.GetDirectoryName(path);
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var backupPath = Path.Combine(directory, filenameWithoutExtension);
            File.Move($@"{backupPath} Backup.exe", path);
            Voodoo2.TryRemoveDgVoodoo(path);
            new Images(new PatchOptions { Path = path }).RemoveGraphics();
        }
    }
}
