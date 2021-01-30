using Serilog;
using Sims1WidescreenPatcher.IO;
using System.IO;
using System.Reflection;

namespace Sims1WidescreenPatcher.Voodoo
{
    public class Voodoo2
    {
        private readonly string[] _voodoo = { "D3D8.dll", "D3D9.dll", "d3dcompiler_47.dll", "D3DImm.dll", "DDraw.dll", "dgVoodoo.conf", "dgVoodooCpl.exe" };
        private readonly string _path;

        public Voodoo2(string path)
        {
            _path = path;
        }

        public void ExtractVoodoo()
        {
            string directory = Path.GetDirectoryName(_path);
            TryRemoveDgVoodoo();
            foreach (var item in _voodoo)
            {
                Log.Debug($"Extract {item} to {_path}.");
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Sims1WidescreenPatcher.Voodoo.DgVoodoo2Resources.{item}"))
                using (var fs = File.Create(Path.Combine(directory, item)))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        public void TryRemoveDgVoodoo()
        {
            Log.Debug("Removing DgVoodoo2 installation.");
            var directory = Path.GetDirectoryName(_path);
            foreach (var item in _voodoo)
                FileHelper.DeleteFile(Path.Combine(directory, item));
        }
    }
}
