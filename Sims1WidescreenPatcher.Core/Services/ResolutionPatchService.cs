using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services;

public class ResolutionPatchService
{
    private IAppState _appState;
    private const string ResolutionPattern = "20 03 ?? ?? ?? ?? ?? 58 02";
    private readonly IPatchFileService _patchFileService;

    public ResolutionPatchService(IAppState appState, IPatchFileService patchFileService)
    {
        _appState = appState;
        _patchFileService = patchFileService;
    }

    public bool CanPatchResolution()
    {
        var (resolutionPatternFound, _, _) = _patchFileService.FindPattern(_appState.SimsExePath, ResolutionPattern);
        return resolutionPatternFound;
    }

    private void EditSimsExe()
    {
        var (found, offset, bytes) = _patchFileService.FindPattern(_appState.SimsExePath, ResolutionPattern);
        if (!found)
        {
            return;
        }

        var widthBytes = BitConverter.GetBytes(_appState.Resolution.Width);
        bytes![offset] = widthBytes[0];
        bytes[offset + 1] = widthBytes[1];
        
        var heightBytes = BitConverter.GetBytes(_appState.Resolution.Height);
        bytes![offset + 2 + 5] = heightBytes[0];
        bytes[offset + 2 + 5 + 1] = heightBytes[1];

        _patchFileService.WriteChanges(_appState.SimsExePath, bytes);
    }
}