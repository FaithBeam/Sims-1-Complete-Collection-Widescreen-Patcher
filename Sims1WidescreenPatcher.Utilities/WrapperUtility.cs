using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Serilog;
using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Utilities;

public static class WrapperUtility
{
    private static string[] _ddrawCompat040Resources = { @"DDrawCompat._0._4._0.ddraw.dll" };
    private static string[] _ddrawCompat032Resources = { @"DDrawCompat._0._3._2.ddraw.dll" };
    private static string[] _dgvoodooResources = { @"DgVoodoo2.D3D8.dll", @"DgVoodoo2.D3DImm.dll", @"DgVoodoo2.DDraw.dll", @"DgVoodoo2.dgVoodoo.conf", @"DgVoodoo2.dgVoodooCpl.exe" };

    public static List<IWrapper> GetWrappers()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            /* Return different ddrawcompat versions depending on Windows version.
               Windows 10 and newer get ddrawcompat 0.4.0
               Windows 8.1 and older get ddrawcompat 0.3.2
             */
            return Environment.OSVersion.Version.Major >= 10 ?
                new List<IWrapper> { new DDrawCompatWrapper("0.4.0"), new DgVoodoo2Wrapper(), new NoneWrapper() } :
                new List<IWrapper> { new DDrawCompatWrapper("0.3.2"), new DgVoodoo2Wrapper(), new NoneWrapper() };
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
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var resourceStream = "Sims1WidescreenPatcher.Utilities.Resources.";
        var resources = new string[] { };

        switch (wrapper)
        {
            case DDrawCompatWrapper w:
                if (w.Version == "0.4.0")
                {
                    resources = _ddrawCompat040Resources;
                }
                else
                {
                    resources = _ddrawCompat032Resources;
                }
                break;
            case DgVoodoo2Wrapper:
                resources = _dgvoodooResources;
                break;
            case NoneWrapper:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(wrapper), wrapper, "");
        }

        foreach (var resource in resources)
        {
            var currentResource = $"{resourceStream}{resource}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(currentResource);
            var resourceSplit = resource.Split('.');
            var combined = resourceSplit[resourceSplit.Length - 2] + "." + resourceSplit[resourceSplit.Length - 1];
            var dest = Path.Combine(dir, combined);
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
        foreach (var item in _ddrawCompat040Resources.Concat(_ddrawCompat032Resources).Concat(_dgvoodooResources))
        {
            var itemSplit = item.Split('.');
            var combined = itemSplit[itemSplit.Length - 2] + "." + itemSplit[itemSplit.Length - 1];
            var delete = Path.Combine(dir, combined);
            File.Delete(delete);
            Log.Debug("Delete {@File}", delete);
        }
        Log.Information("End remove wrapper");
    }
}