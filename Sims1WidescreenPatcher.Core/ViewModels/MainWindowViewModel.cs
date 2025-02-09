﻿using ReactiveUI;
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface IMainWindowViewModel
{
    IMainTabViewModel? MainTabViewModel { get; }
    IExtrasTabViewModel? ExtrasTabViewModel { get; }
    INotificationViewModel NotificationViewModel { get; }
}

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(
        IMainTabViewModel mainTabViewModel,
        IExtrasTabViewModel extrasTabViewModel,
        INotificationViewModel notificationViewModel
    )
    {
        MainTabViewModel = mainTabViewModel;
        ExtrasTabViewModel = extrasTabViewModel;
        NotificationViewModel = notificationViewModel;
    }

    public IMainTabViewModel? MainTabViewModel { get; }
    public IExtrasTabViewModel? ExtrasTabViewModel { get; }
    public INotificationViewModel NotificationViewModel { get; }
}
