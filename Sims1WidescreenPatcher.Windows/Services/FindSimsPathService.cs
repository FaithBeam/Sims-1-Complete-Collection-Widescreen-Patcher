using System;
using System.IO;
using System.Runtime.Versioning;
using Microsoft.Win32;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Windows.Services;

[SupportedOSPlatform("windows")]
public class FindSimsPathService : IFindSimsPathService
{
    public string FindSimsPath()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\WOW6432Node\Maxis\The Sims"
            );
            var val = key?.GetValue("InstallPath")?.ToString();
            if (string.IsNullOrWhiteSpace(val))
            {
                return string.Empty;
            }
            var path = Path.Combine(val, "Sims.exe");
            return File.Exists(path) ? path : string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
