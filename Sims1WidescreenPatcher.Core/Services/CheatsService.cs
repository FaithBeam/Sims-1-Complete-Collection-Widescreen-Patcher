using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class CheatsService(IAppState appState, IPatchFileService patchFileService) : ICheatsService
{
    private const string DisableCheatsPattern = "00 56 90 90";
    private const string EnableCheatsPattern = "00 56 75 04";

    public bool CheatsEnabled()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return false;
        }
        var (found, _, _) = patchFileService.FindPattern(appState.SimsExePath, DisableCheatsPattern);
        return found;
    }

    /// <summary>
    /// Determine if the sims exe can be patched to enable all cheats
    /// </summary>
    /// <returns></returns>
    public bool CanEnableCheats()
    {
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return false;
        }
        var (disablePatternFound, _, _) = patchFileService.FindPattern(appState.SimsExePath, DisableCheatsPattern);
        var (enablePatternFound, _, _) = patchFileService.FindPattern(appState.SimsExePath,EnableCheatsPattern);
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
        if (string.IsNullOrWhiteSpace(appState.SimsExePath))
        {
            return;
        }
        var (found, offset, bytes) = patchFileService.FindPattern(appState.SimsExePath, pattern);
        if (!found)
        {
            return;
        }

        bytes![offset + 2] = replacementBytes.Item1;
        bytes[offset + 2 + 1] = replacementBytes.Item2;

        patchFileService.WriteChanges(appState.SimsExePath, bytes);
    }
}