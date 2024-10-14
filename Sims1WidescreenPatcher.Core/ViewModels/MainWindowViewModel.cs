using ReactiveUI;
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class MainWindowViewModel(
    IMainTabViewModel mainTabViewModel,
    IExtrasTabViewModel extrasTabViewModel,
    INotificationViewModel notificationViewModel)
    : ViewModelBase, IMainWindowViewModel
{
    public IMainTabViewModel? MainTabViewModel { get; } = mainTabViewModel;
    public IExtrasTabViewModel? ExtrasTabViewModel { get; } = extrasTabViewModel;
    public INotificationViewModel NotificationViewModel { get; } = notificationViewModel;
}