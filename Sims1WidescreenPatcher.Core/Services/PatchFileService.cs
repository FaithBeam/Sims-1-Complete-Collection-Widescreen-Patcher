using PatternFinder;

namespace Sims1WidescreenPatcher.Core.Services;

public class PatchFileService : IPatchFileService
{
    public void WriteChanges(string path, byte[] bytes)
    {
        File.SetAttributes(path, FileAttributes.Normal);
        File.WriteAllBytes(path, bytes);
    }
    
    public (bool found, long offset, byte[]? bytes) FindPattern(string path, string pattern)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return (false, 0, null);
        }

        var patternBytes = Pattern.Transform(pattern);
        var bytes = File.ReadAllBytes(path);
        if (!Pattern.Find(bytes, patternBytes, out var offset))
        {
            return (false, 0, null);
        }

        return (true, offset, bytes);
    }
}