using System.Reflection;
using System.Runtime.InteropServices;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services;

public class WrapperService : IWrapperService
{
    private readonly IAppState _appState;

    public WrapperService(IAppState appState)
    {
        _appState = appState;
    }

    private static string[] _ddrawCompat054Resources = { @"DDrawCompat._0._5._4.ddraw.dll" };
    private static string[] _ddrawCompat032Resources = { @"DDrawCompat._0._3._2.ddraw.dll" };

    private static string[] _dgvoodooResources =
    {
        @"DgVoodoo2.D3D8.dll",
        @"DgVoodoo2.D3DImm.dll",
        @"DgVoodoo2.DDraw.dll",
        @"DgVoodoo2.dgVoodoo.conf",
        @"DgVoodoo2.dgVoodooCpl.exe",
    };

    public List<IWrapper> GetWrappers()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            /* Return different ddrawcompat versions depending on Windows version.
               Windows 10 and newer get ddrawcompat 0.5.4
               Windows 8.1 and older get ddrawcompat 0.3.2
             */
            return Environment.OSVersion.Version.Major >= 10
                ? new List<IWrapper>
                {
                    new DDrawCompatWrapper("0.5.4"),
                    new DgVoodoo2Wrapper(),
                    new NoneWrapper(),
                }
                : new List<IWrapper>
                {
                    new DDrawCompatWrapper("0.3.2"),
                    new DgVoodoo2Wrapper(),
                    new NoneWrapper(),
                };
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new List<IWrapper> { new NoneWrapper(), new DgVoodoo2Wrapper() };
        }

        return new List<IWrapper> { new NoneWrapper() };
    }

    public async Task Install(IWrapper wrapper)
    {
        var simsInstallDir = GetSimsInstallDirectory();
        const string resourceStream = "Sims1WidescreenPatcher.Core.Resources.";
        var resources = Array.Empty<string>();

        switch (wrapper)
        {
            case DDrawCompatWrapper w:
                resources =
                    w.Version == "0.5.4" ? _ddrawCompat054Resources : _ddrawCompat032Resources;
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
            await using var stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(currentResource);
            var resourceSplit = resource.Split('.');
            var combined = resourceSplit[^2] + "." + resourceSplit[^1];
            var dest = Path.Combine(simsInstallDir, combined);
            await using var fs = File.Create(dest);
            await stream!.CopyToAsync(fs);
        }
    }

    public void Uninstall()
    {
        var simsInstallDir = GetSimsInstallDirectory();
        foreach (
            var item in _ddrawCompat054Resources
                .Concat(_ddrawCompat032Resources)
                .Concat(_dgvoodooResources)
        )
        {
            var itemSplit = item.Split('.');
            var combined = itemSplit[^2] + "." + itemSplit[^1];
            var delete = Path.Combine(simsInstallDir, combined);
            File.Delete(delete);
        }
    }

    private string GetSimsInstallDirectory()
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            throw new Exception("Sims exe path is null");
        }

        return Path.GetDirectoryName(_appState.SimsExePath)
            ?? throw new Exception($"Failed to get directory name of {_appState.SimsExePath}");
    }
}
