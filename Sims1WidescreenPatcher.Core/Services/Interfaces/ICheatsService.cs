namespace Sims1WidescreenPatcher.Core.Services.Interfaces;

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
