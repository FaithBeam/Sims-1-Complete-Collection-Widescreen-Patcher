using PatternFinder;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services;

public class CheatsService : ICheatsService
{
    private const string DisableCheatsPattern = "00 56 90 90";
    private const string EnableCheatsPattern = "00 56 75 04";
    private readonly IAppState _appState;

    public CheatsService(IAppState appState)
    {
        _appState = appState;
    }

    public bool CheatsEnabled()
    {
        var (found, _, _) = FindPattern(DisableCheatsPattern);
        return found;
    }

    public void EnableCheats()
    {
        EditSimsExe( EnableCheatsPattern, new Tuple<byte, byte>(144, 144));
    }

    public void DisableCheats()
    {
        EditSimsExe( DisableCheatsPattern, new Tuple<byte, byte>(117, 4));
    }

    private void EditSimsExe(string pattern, Tuple<byte, byte> replacementBytes)
    {
        var (found, offset, bytes) = FindPattern(pattern);
        if (!found)
        {
            return;
        }

        bytes![offset + 2] = replacementBytes.Item1;
        bytes[offset + 2 + 1] = replacementBytes.Item2;
        
        WriteChanges(bytes);
    }

    private void WriteChanges(byte[] bytes)
    {
        File.SetAttributes(_appState.SimsExePath, FileAttributes.Normal);
        File.WriteAllBytes(_appState.SimsExePath, bytes);
    }

    private (bool found, long offset, byte[]? bytes) FindPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath) || !File.Exists(_appState.SimsExePath))
        {
            return (false, 0, null);
        }

        var patternBytes = Pattern.Transform(pattern);
        var bytes = File.ReadAllBytes(_appState.SimsExePath);
        if (!Pattern.Find(bytes, patternBytes, out var offset))
        {
            return (false, 0, null);
        }

        return (true, offset, bytes);
    }
}