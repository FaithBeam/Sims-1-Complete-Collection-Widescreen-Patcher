using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IWrapperService
{
    List<IWrapper> GetWrappers();
    Task Install(IWrapper wrapper);
    void Uninstall();
}