using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Serilog;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Wrappers.Models;

namespace Sims1WidescreenPatcher.Wrappers
{
    public static class GraphicsWrapper
    {
        private static readonly List<string> _ddrawCompatResources = new List<string>() {"DDraw.dll"};

        private static readonly List<string> _dgvoodooResources = new List<string>()
        {
            "D3D8.dll", "D3D9.dll", "D3DImm.dll", "DDraw.dll", "dgVoodoo.conf", "dgVoodooCpl.exe"
        };

        public static void CopyDll(Wrapper wrapper, string path)
        {
            var directory = Path.GetDirectoryName(path);
            var resourceStream = "Sims1WidescreenPatcher.Wrappers.Resources.";
            var resources = new List<string>();

            switch (wrapper)
            {
                case Wrapper.DDrawCompat:
                    resourceStream += "DDrawCompat";
                    resources = _ddrawCompatResources;
                    break;
                case Wrapper.DgVoodoo2:
                    resourceStream += "DgVoodoo2";
                    resources = _dgvoodooResources;
                    break;
                case Wrapper.None:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wrapper), wrapper, null);
            }

            foreach (var resource in resources)
            {
                var currentResource = $"{resourceStream}.{resource}";
                Log.Debug("Extract {Resource} to {Path}", resource, path);
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(currentResource))
                using (var fs = File.Create(Path.Combine(directory, resource)))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        public static void TryRemoveWrapper(string path)
        {
            Log.Debug("Removing graphics wrapper installation");
            var directory = Path.GetDirectoryName(path);
            foreach (var item in _ddrawCompatResources)
                FileHelper.DeleteFile(Path.Combine(directory, item));
            foreach (var item in _dgvoodooResources)
                FileHelper.DeleteFile(Path.Combine(directory, item));
        }
    }
}