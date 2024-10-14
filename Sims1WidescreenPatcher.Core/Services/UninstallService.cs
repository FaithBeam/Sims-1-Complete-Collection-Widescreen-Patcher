using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class UninstallService(
    IAppState appState,
    IImagesService imagesService,
    IProgressService progressService,
    IWrapperService wrapperService)
    : IUninstallService
{
    public void Uninstall()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return;
        }
        if (appState.SimsBackupExists)
        {
            File.SetAttributes(appState.SimsExePath, FileAttributes.Normal);
            File.Delete(appState.SimsExePath);
            File.Move(appState.SimsBackupPath, appState.SimsExePath);

            wrapperService.Uninstall();

            imagesService.Uninstall();
            
            progressService.UpdateUninstall();
        }
        else
        {
            throw new Exception($"Sims exe path doesn't exist");
        }
    }
}