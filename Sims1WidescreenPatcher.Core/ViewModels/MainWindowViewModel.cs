using ReactiveUI;
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface IMainWindowViewModel
{
    IMainTabViewModel? MainTabViewModel { get; }
    IExtrasTabViewModel? ExtrasTabViewModel { get; }
    ICareerEditorTabViewModel? CareerEditorTabViewModel { get; }
    INotificationViewModel NotificationViewModel { get; }
}

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    public MainWindowViewModel(
        IMainTabViewModel mainTabViewModel,
        IExtrasTabViewModel extrasTabViewModel,
        ICareerEditorTabViewModel careerEditorTabViewModel,
        INotificationViewModel notificationViewModel
    )
    {
        MainTabViewModel = mainTabViewModel;
        ExtrasTabViewModel = extrasTabViewModel;
        CareerEditorTabViewModel = careerEditorTabViewModel;
        NotificationViewModel = notificationViewModel;
    }

    public IMainTabViewModel? MainTabViewModel { get; }
    public IExtrasTabViewModel? ExtrasTabViewModel { get; }
    public ICareerEditorTabViewModel? CareerEditorTabViewModel { get; }
    public INotificationViewModel NotificationViewModel { get; }
}
