using PatternFinder;

namespace Sims1WidescreenPatcher.Core.Services;

public class CheatsService : ICheatsService
{
    private const string DisableCheatsPattern = "00 56 90 90";
    private const string EnableCheatsPattern = "00 56 75 04";

    public bool CheatsEnabled(string simsExePath)
    {
        var (found, _, _) = FindPattern(simsExePath, DisableCheatsPattern);
        return found;
    }

    public void EnableCheats(string simsExePath)
    {
        EditSimsExe(simsExePath, EnableCheatsPattern, new Tuple<byte, byte>(144, 144));
    }

    public void DisableCheats(string simsExePath)
    {
        EditSimsExe(simsExePath, DisableCheatsPattern, new Tuple<byte, byte>(117, 4));
    }

    private void EditSimsExe(string simsExePath, string pattern, Tuple<byte, byte> replacementBytes)
    {
        var (found, offset, bytes) = FindPattern(simsExePath, pattern);
        if (!found)
        {
            return;
        }

        bytes![offset + 2] = replacementBytes.Item1;
        bytes[offset + 2 + 1] = replacementBytes.Item2;
        
        WriteChanges(simsExePath, bytes);
    }

    private void WriteChanges(string simsExePath, byte[] bytes)
    {
        File.SetAttributes(simsExePath, FileAttributes.Normal);
        File.WriteAllBytes(simsExePath, bytes);
    }

    private (bool found, long offset, byte[]? bytes) FindPattern(string simsExePath, string pattern)
    {
        if (!File.Exists(simsExePath))
        {
            return (false, 0, null);
        }

        var patternBytes = Pattern.Transform(pattern);
        var bytes = File.ReadAllBytes(simsExePath);
        if (!Pattern.Find(bytes, patternBytes, out var offset))
        {
            throw new Exception($"Could not find pattern {EnableCheatsPattern} in {simsExePath}");
        }

        return (true, offset, bytes);
    }
}