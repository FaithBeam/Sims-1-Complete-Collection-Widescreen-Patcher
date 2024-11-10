namespace Sims1WidescreenPatcher.Core.Services.Interfaces;

public interface IPatchFileService
{
    void WriteChanges(string simsExePath, byte[] bytes);
    (bool found, long offset, byte[]? bytes) FindPattern(string simsExePath, string pattern);
}
