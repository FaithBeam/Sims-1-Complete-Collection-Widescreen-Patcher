using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.Services.Interfaces;

public interface IResolutionsService
{
    IEnumerable<Resolution> GetResolutions();
}