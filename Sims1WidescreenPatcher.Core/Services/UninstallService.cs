using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class UninstallService : IUninstallService
{
    private readonly IAppState _appState;
    private readonly IImagesService _imagesService;
    private readonly IProgressService _progressService;
    private readonly IWrapperService _wrapperService;

    public UninstallService(
        IAppState appState,
        IImagesService imagesService,
        IProgressService progressService,
        IWrapperService wrapperService
    )
    {
        _appState = appState;
        _imagesService = imagesService;
        _progressService = progressService;
        _wrapperService = wrapperService;
    }

    public void Uninstall()
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            return;
        }
        if (_appState.SimsBackupExists)
        {
            File.SetAttributes(_appState.SimsExePath, FileAttributes.Normal);
            File.Delete(_appState.SimsExePath);
            File.Move(_appState.SimsBackupPath, _appState.SimsExePath);

            _wrapperService.Uninstall();

            _imagesService.Uninstall();

            _progressService.UpdateUninstall();
        }
        else
        {
            throw new Exception($"Sims exe path doesn't exist");
        }
    }
}
