using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using Sims1WidescreenPatcher.Utilities;

namespace Sims1WidescreenPatcher.Core.Services;

public class UninstallService : IUninstallService
{
    private readonly IAppState _appState;
    private readonly IImagesService _imagesService;

    public UninstallService(IAppState appState, IImagesService imagesService)
    {
        _appState = appState;
        _imagesService = imagesService;
    }

    public void Uninstall()
    {
        if (_appState.SimsBackupExists)
        {
            File.SetAttributes(_appState.SimsExePath, FileAttributes.Normal);
            File.Delete(_appState.SimsExePath);
            File.Move(_appState.SimsBackupPath, _appState.SimsExePath);
            
            WrapperUtility.RemoveWrapper(_appState.SimsExePath);
            
            _imagesService.Uninstall();
        }
        else
        {
            throw new Exception($"Sims exe path doesn't exist");
        }
    }
}