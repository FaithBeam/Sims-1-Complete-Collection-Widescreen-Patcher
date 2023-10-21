using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(IMainTabViewModel mainTabViewModel, IOptionalTabViewModel optionalTabViewModel)
    {
        MainTabViewModel = mainTabViewModel;
        OptionalTabViewModel = optionalTabViewModel;
    }

    public IMainTabViewModel? MainTabViewModel { get; }
    public IOptionalTabViewModel? OptionalTabViewModel { get; }
}