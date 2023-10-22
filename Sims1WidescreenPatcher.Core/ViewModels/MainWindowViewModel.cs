using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(IMainTabViewModel mainTabViewModel, IExtrasTabViewModel extrasTabViewModel)
    {
        MainTabViewModel = mainTabViewModel;
        ExtrasTabViewModel = extrasTabViewModel;
    }

    public IMainTabViewModel? MainTabViewModel { get; }
    public IExtrasTabViewModel? ExtrasTabViewModel { get; }
}