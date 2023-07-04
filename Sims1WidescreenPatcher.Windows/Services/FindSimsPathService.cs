using System;
using System.IO;
using Microsoft.Win32;
using Sims1WidescreenPatcher.Core.Services;

namespace Sims1WidescreenPatcher.Windows.Services;

public class FindSimsPathService : IFindSimsPathService
{
    public string FindSimsPath()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Maxis\The Sims");
            var val = key.GetValue("InstallPath").ToString();
            var path = Path.Combine(val, "Sims.exe");
            return path;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}