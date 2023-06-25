using System.Reflection;
using System.Runtime.InteropServices;
using Serilog;
using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Utilities;

public static class WrapperUtility
{
    private static string[] _ddrawCompatResources = { "ddraw.dll" };

    private static string[] _dgvoodooResources =
        {"D3D8.dll", "D3DImm.dll", "DDraw.dll", "dgVoodoo.conf", "dgVoodooCpl.exe"};

    public static List<IWrapper> GetWrappers()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            /* Return different ddrawcompat versions depending on Windows version.
               Windows 10 and newer get ddrawcompat 0.4.0
               Windows 8.1 and older get ddrawcompat 0.4.0+win7.fix
             */
            return Environment.OSVersion.Version.Major >= 10 ?
                new List<IWrapper> { new DDrawCompatWrapper("0.4.0"), new DgVoodoo2Wrapper(), new NoneWrapper() } :
                new List<IWrapper> { new DDrawCompatWrapper("0.4.0+win7.fix"), new DgVoodoo2Wrapper(), new NoneWrapper() };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new List<IWrapper> { new NoneWrapper(), new DgVoodoo2Wrapper() };
        }
        else
        {
            return new List<IWrapper> { new NoneWrapper() };
        }
    }

    public static async Task ExtractWrapper(IWrapper wrapper, string path)
    {
        Log.Information("Begin extract wrapper");
        Log.Debug("Wrapper {@Wrapper}", wrapper);
        Log.Debug("Path {@Path}", path);
        System.Diagnostics.Debug.WriteLine(Assembly.GetExecutingAssembly().FullName);
        System.Diagnostics.Debug.WriteLine(Assembly.GetExecutingAssembly().GetManifestResourceNames());
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var resourceStream = "Sims1WidescreenPatcher.Utilities.Resources.";
        string[] resources;

        
        switch (wrapper)
        {
            case DDrawCompatWrapper w:
                if (w.Version == "0.4.0")
                {
                    resourceStream += @"DDrawCompat";
                }
                else
                {
                    resourceStream += @"DDrawCompat\fix";
                }
                resources = _ddrawCompatResources;
                break;
            case DgVoodoo2Wrapper:
                resourceStream += "DgVoodoo2";
                resources = _dgvoodooResources;
                break;
            case NoneWrapper:
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(wrapper), wrapper, null);
        }

        foreach (var resource in resources)
        {
            var currentResource = $"{resourceStream}.{resource}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(currentResource);
            var dest = Path.Combine(dir, resource);
            using var fs = File.Create(dest);
            await stream!.CopyToAsync(fs);
            Log.Debug("Copied {@Resource} to {@Path}", resource, dest);
        }
        Log.Information("End extract wrapper");
    }

    public static void RemoveWrapper(string path)
    {
        Log.Information("Begin remove wrapper");
        Log.Debug("Path {@Path}", path);
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        foreach (var item in _ddrawCompatResources.Concat(_dgvoodooResources))
        {
            var delete = Path.Combine(dir, item);
            File.Delete(delete);
            Log.Debug("Delete {@File}", delete);
        }
        Log.Information("End remove wrapper");
    }
}