using PatternFinder;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class PatchFileService : IPatchFileService
{
    public void WriteChanges(string simsExePath, byte[] bytes)
    {
        File.SetAttributes(simsExePath, FileAttributes.Normal);
        File.WriteAllBytes(simsExePath, bytes);
    }
    
    public (bool found, long offset, byte[]? bytes) FindPattern(string simsExePath, string pattern)
    {
        if (string.IsNullOrWhiteSpace(simsExePath) || !File.Exists(simsExePath))
        {
            return (false, 0, null);
        }

        var patternBytes = Pattern.Transform(pattern);
        var bytes = File.ReadAllBytes(simsExePath);
        if (!Pattern.Find(bytes, patternBytes, out var offset))
        {
            return (false, 0, null);
        }

        return (true, offset, bytes);
    }
}