namespace Sims1WidescreenPatcher.Core.Services;

public interface ICheatsService
{
    bool CheatsEnabled(string simsExePath);
    void EnableCheats(string simsExePath);
    void DisableCheats(string simsExePath);
}