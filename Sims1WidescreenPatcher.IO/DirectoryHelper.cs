using System.IO;
using Serilog;

namespace Sims1WidescreenPatcher.IO
{
    public static class DirectoryHelper
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Log.Debug("Creating {Path}", path);
                Directory.CreateDirectory(path);
            }
            else
            {
                Log.Debug("{Path} already exists, not creating", path);
            }
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Log.Debug("Deleting {Path}", path);
                Directory.Delete(path, true);
            }
            else
            {
                Log.Debug("{Path} does not exist, not deleting", path);
            }
        }
    }
}
