using Serilog;
using System.IO;

namespace Sims1WidescreenPatcher.IO
{
    public static class DirectoryHelper
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Log.Debug($"Creating {path}.");
                Directory.CreateDirectory(path);
            }
            else
            {
                Log.Debug($"{path} already exists, not creating.");
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Log.Debug($"Deleting {path}.");
                Directory.Delete(path, true);
            }
            else
            {
                Log.Debug($"{path} does not exist, not deleting.");
            }
        }
    }
}
