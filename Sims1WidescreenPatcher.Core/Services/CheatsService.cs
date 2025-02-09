﻿using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public interface ICheatsService
{
    bool CheatsEnabled();

    /// <summary>
    /// Determine if the sims exe can be patched to enable all cheats
    /// </summary>
    /// <returns></returns>
    bool CanEnableCheats();

    void EnableCheats();
    void DisableCheats();
}

public class CheatsService : ICheatsService
{
    private readonly IAppState _appState;
    private readonly IPatchFileService _patchFileService;

    public CheatsService(IAppState appState, IPatchFileService patchFileService)
    {
        _appState = appState;
        _patchFileService = patchFileService;
    }

    private const string DisableCheatsPattern = "00 56 90 90";
    private const string EnableCheatsPattern = "00 56 75 04";

    public bool CheatsEnabled()
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            return false;
        }
        var (found, _, _) = _patchFileService.FindPattern(
            _appState.SimsExePath,
            DisableCheatsPattern
        );
        return found;
    }

    /// <summary>
    /// Determine if the sims exe can be patched to enable all cheats
    /// </summary>
    /// <returns></returns>
    public bool CanEnableCheats()
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            return false;
        }
        var (disablePatternFound, _, _) = _patchFileService.FindPattern(
            _appState.SimsExePath,
            DisableCheatsPattern
        );
        var (enablePatternFound, _, _) = _patchFileService.FindPattern(
            _appState.SimsExePath,
            EnableCheatsPattern
        );
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
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            return;
        }
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
