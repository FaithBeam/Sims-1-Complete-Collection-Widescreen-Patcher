using Serilog;
using Sims1WidescreenPatcher.IO;
using System.IO;
using System.Reflection;

namespace Sims1WidescreenPatcher.DDrawCompat
{
    public class DllWrapper
    {
        private readonly string[] _dlls = { "DDraw.dll" };
        private readonly string _path;

        public DllWrapper(string path)
        {
            _path = path;
        }

        public void CopyDll()
        {
            string directory = Path.GetDirectoryName(_path);
            TryRemoveDDrawCompat();
            foreach (var item in _dlls)
            {
                Log.Debug($"Extract {item} to {_path}.");
                var f = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Sims1WidescreenPatcher.DDrawCompat.Resources.{item}"))
                using (var fs = File.Create(Path.Combine(directory, item)))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        public void TryRemoveDDrawCompat()
        {
            Log.Debug("Removing DDrawCompat installation.");
            var directory = Path.GetDirectoryName(_path);
            foreach (var item in _dlls)
                FileHelper.DeleteFile(Path.Combine(directory, item));
        }
    }
}
