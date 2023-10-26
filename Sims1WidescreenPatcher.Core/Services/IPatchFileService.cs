namespace Sims1WidescreenPatcher.Core.Services;

public interface IPatchFileService
{
    void WriteChanges(string path, byte[] bytes);
    (bool found, long offset, byte[]? bytes) FindPattern(string path, string pattern);
}