using System.Reflection;
using System.Runtime.InteropServices;
using Serilog;

namespace Sims1WidescreenPatcher.Utilities;

public static class WrapperUtility
{
    public enum Wrapper
    {
        None,
        DDrawCompat,
        DgVoodoo2
    }

    private static string[] _ddrawCompatResources = { "ddraw.dll" };

    private static string[] _dgvoodooResources =
        {"D3D8.dll", "D3DImm.dll", "DDraw.dll", "dgVoodoo.conf", "dgVoodooCpl.exe"};

    public static List<Wrapper> GetWrappers()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new List<Wrapper> { Wrapper.DDrawCompat, Wrapper.DgVoodoo2, Wrapper.None };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new List<Wrapper> { Wrapper.None, Wrapper.DgVoodoo2 };
        }
        else
        {
            return new List<Wrapper> { Wrapper.None };
        }
    }

    public static async Task ExtractWrapper(Wrapper wrapper, string path)
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