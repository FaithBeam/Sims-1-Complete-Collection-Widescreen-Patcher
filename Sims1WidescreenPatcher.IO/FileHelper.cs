using System.IO;
using Serilog;

namespace Sims1WidescreenPatcher.IO
{
    public static class FileHelper
    {
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                Log.Debug("Deleting {Path}", path);
                File.Delete(path);
            }
            else
            {
                Log.Debug("{Path} doesn't exist", path);
            }
        }

        public static void BackupFile(string path)
        {
            var filename = Path.GetFileNameWithoutExtension(path);
            var directory = Path.GetDirectoryName(path);
            if (!File.Exists($@"{directory}\{filename} Backup.exe"))
            {
                Log.Debug(@"Backing up {Path} to {Directory}\{Filename} Backup.exe", path, directory, filename);
                File.Copy(path, $@"{directory}\{filename} Backup.exe");
                File.SetAttributes($@"{directory}\{filename} Backup.exe", FileAttributes.Normal);
            }
            else
            {
                Log.Debug(@"There is already a backup at {Directory}\{Filename} Backup.exe, not creating another", directory, filename);
            }
        }

        public static bool CheckForBackup(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var exists = File.Exists($@"{directory}\{fileName} Backup.exe");
            Log.Debug("{Path} exists: {Exists}", path, exists);
            return File.Exists($@"{directory}\{fileName} Backup.exe");
        }
    }
}
