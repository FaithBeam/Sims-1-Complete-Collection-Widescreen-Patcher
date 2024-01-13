using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class CheatsService : ICheatsService
{
    private const string DisableCheatsPattern = "00 56 90 90";
    private const string EnableCheatsPattern = "00 56 75 04";
    private readonly IAppState _appState;
    private readonly IPatchFileService _patchFileService;

    public CheatsService(IAppState appState, IPatchFileService patchFileService)
    {
        _appState = appState;
        _patchFileService = patchFileService;
    }

    public bool CheatsEnabled()
    {
        var (found, _, _) = _patchFileService.FindPattern(_appState.SimsExePath, DisableCheatsPattern);
        return found;
    }

    /// <summary>
    /// Determine if the sims exe can be patched to enable all cheats
    /// </summary>
    /// <returns></returns>
    public bool CanEnableCheats()
    {
        var (disablePatternFound, _, _) = _patchFileService.FindPattern(_appState.SimsExePath, DisableCheatsPattern);
        var (enablePatternFound, _, _) = _patchFileService.FindPattern(_appState.SimsExePath,EnableCheatsPattern);
        return disablePatternFound || enablePatternFound;
    }

    public void EnableCheats()
    {
        EditSimsExe(EnableCheatsPattern, new Tuple<byte, byte>(144, 144));
    }

    public void DisableCheats()
    {
        EditSimsExe(DisableCheatsPattern, new Tuple<byte, byte>(117, 4));
    }

    private void EditSimsExe(string pattern, Tuple<byte, byte> replacementBytes)
    {
        var (found, offset, bytes) = _patchFileService.FindPattern(_appState.SimsExePath, pattern);
        if (!found)
        {
            return;
        }

        bytes![offset + 2] = replacementBytes.Item1;
        bytes[offset + 2 + 1] = replacementBytes.Item2;

        _patchFileService.WriteChanges(_appState.SimsExePath, bytes);
    }
}