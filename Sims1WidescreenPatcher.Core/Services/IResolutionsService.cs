using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IResolutionsService
{
    IEnumerable<Resolution> GetResolutions();
}