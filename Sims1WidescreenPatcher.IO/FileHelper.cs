using Serilog;
using System.IO;

namespace Sims1WidescreenPatcher.IO
{
    public static class FileHelper
    {
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                Log.Debug($"Deleting {path}");
                File.Delete(path);
            }
            else
            {
                Log.Debug($"{path} doesn't exist");
            }
        }

        public static void BackupFile(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            if (!File.Exists($@"{directory}\{filename} Backup.exe"))
            {
                Log.Debug($@"Backing up {path} to {directory}\{filename} Backup.exe.");
                File.Copy(path, $@"{directory}\{filename} Backup.exe");
                File.SetAttributes($@"{directory}\{filename} Backup.exe", FileAttributes.Normal);
            }
            else
            {
                Log.Debug($@"There is already a backup at {directory}\{filename} Backup.exe, not creating another.");
            }
        }

        public static bool CheckForBackup(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            Log.Debug($"{path} exists: {File.Exists($@"{directory}\{fileName} Backup.exe")}.");
            return File.Exists($@"{directory}\{fileName} Backup.exe");
        }
    }
}
