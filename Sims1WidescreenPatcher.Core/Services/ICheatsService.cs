namespace Sims1WidescreenPatcher.Core.Services;

public interface ICheatsService
{
    bool CheatsEnabled();
    void EnableCheats();
    void DisableCheats();
}