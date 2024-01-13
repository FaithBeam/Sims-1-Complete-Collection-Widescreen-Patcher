using Avalonia.Input;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class MainWindow : ReactiveWindow<IMainWindowViewModel>
{
    private INotificationViewModel? _notificationViewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            _notificationViewModel = ((IMainWindowViewModel)DataContext!).NotificationViewModel;
        });
    }

    private void MainWindowPanel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_notificationViewModel is not null && _notificationViewModel.HasBeenShown)
        {
            _notificationViewModel.IsVisible = false;
        }
    }

    private void ExtrasTabItem_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_notificationViewModel is not null && !_notificationViewModel.HasBeenShown)
        {
            _notificationViewModel.IsVisible = true;
            _notificationViewModel.HasBeenShown = true;
        }
    }

    private void TabControl_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_notificationViewModel is not null && _notificationViewModel.HasBeenShown)
        {
            _notificationViewModel.IsVisible = false;
        }
    }

    private void MainTabItem_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_notificationViewModel is not null && _notificationViewModel.HasBeenShown)
        {
            _notificationViewModel.IsVisible = false;
        }
    }
}