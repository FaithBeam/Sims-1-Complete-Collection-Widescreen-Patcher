using System.IO;

namespace Sims1WidescreenPatcher.IO
{
    public static class FileHelper
    {
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void BackupFile(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            if (!File.Exists($@"{directory}\{filename} Backup.exe"))
            {
                File.Copy(path, $@"{directory}\{filename} Backup.exe");
                File.SetAttributes($@"{directory}\{filename} Backup.exe", FileAttributes.Normal);
            }
        }

        public static bool CheckForBackup(string path)
        {
            string directory = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            return File.Exists($@"{directory}\{fileName} Backup.exe");
        }
    }
}
