using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class ResolutionPatchService(IAppState appState, IPatchFileService patchFileService) : IResolutionPatchService
{
    private const string ResolutionPattern = "20 03 ?? ?? ?? ?? ?? 58 02";

    public bool CanPatchResolution()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return false;
        }
        var (resolutionPatternFound, _, _) = patchFileService.FindPattern(appState.SimsExePath, ResolutionPattern);
        return resolutionPatternFound;
    }

    public void EditSimsExe()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath) || appState.Resolution is null)
        {
            return;
        }
        var (found, offset, bytes) = patchFileService.FindPattern(appState.SimsExePath, ResolutionPattern);
        if (!found || bytes is null)
        {
            return;
        }

        var widthBytes = BitConverter.GetBytes(appState.Resolution.Width);
        bytes[offset] = widthBytes[0];
        bytes[offset + 1] = widthBytes[1];
        
        var heightBytes = BitConverter.GetBytes(appState.Resolution.Height);
        bytes[offset + 2 + 5] = heightBytes[0];
        bytes[offset + 2 + 5 + 1] = heightBytes[1];

        patchFileService.WriteChanges(appState.SimsExePath, bytes);
    }
    
    public bool BackupExists()
    {
        var backupPath = GetSimsBackupPath();
        if (!string.IsNullOrWhiteSpace(backupPath))
        {
            return File.Exists(GetSimsBackupPath());
        }

        return false;
    }

    public void CreateBackup()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return;
        }
        var backupPath = GetSimsBackupPath();
        if (!string.IsNullOrWhiteSpace(backupPath) && !File.Exists(backupPath))
        {
            File.Copy(appState.SimsExePath, backupPath);
            File.SetAttributes(backupPath, FileAttributes.Normal);
        }
    }

    private string GetSimsBackupPath()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return "";
        }
        const string backupName = "Sims Backup.exe";
        var parent = Directory.GetParent(appState.SimsExePath)!.ToString();
        return Path.Combine(parent, backupName);
    }
}