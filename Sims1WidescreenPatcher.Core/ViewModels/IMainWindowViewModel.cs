
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface IMainWindowViewModel
{
    IMainTabViewModel? MainTabViewModel { get; }
    IOptionalTabViewModel? OptionalTabViewModel { get; }
}