using System.Collections.Generic;
using Serilog;
using Sims1WidescreenPatcher.IO;
using System.IO;
using System.Reflection;
using Sims1WidescreenPatcher.Wrappers.Models;

namespace Sims1WidescreenPatcher.Wrappers
{
    public class DllWrapper
    {
        private readonly List<string> _ddrawCompatResources = new List<string>() {"DDraw.dll"};

        private readonly List<string> _dgvoodooResources = new List<string>()
        {
            "D3D8.dll", "D3D9.dll", "D3DImm.dll", "DDraw.dll", "dgVoodoo.conf", "dgVoodooCpl.exe"
        };

        private readonly string _path;

        public DllWrapper(string path)
        {
            _path = path;
        }

        public void CopyDll(Wrapper wrapper)
        {
            var directory = Path.GetDirectoryName(_path);
            TryRemoveWrapper();
            var resourceStream = "Sims1WidescreenPatcher.Wrappers.Resources.";
            var items = new List<string>();
            
            switch (wrapper)
            {
                case Wrapper.DDrawCompat:
                    resourceStream += "DDrawCompat";
                    items = _ddrawCompatResources;
                    break;
                case Wrapper.DgVoodoo2:
                    resourceStream += "DgVoodoo2";
                    items = _dgvoodooResources;
                    break;
            }

            foreach (var item in items)
            {
                var currentResource = $"{resourceStream}.{item}";
                Log.Debug($"Extract {item} to {_path}.");
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(currentResource))
                using (var fs = File.Create(Path.Combine(directory, item)))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        public void TryRemoveWrapper()
        {
            Log.Debug("Removing wrapper installation.");
            var directory = Path.GetDirectoryName(_path);
            foreach (var item in _ddrawCompatResources)
                FileHelper.DeleteFile(Path.Combine(directory, item));
            foreach (var item in _dgvoodooResources)
                FileHelper.DeleteFile(Path.Combine(directory, item));
        }
    }
}