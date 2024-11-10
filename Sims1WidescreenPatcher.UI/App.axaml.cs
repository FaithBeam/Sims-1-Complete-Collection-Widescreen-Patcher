using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.UI.Views;
using Splat;

namespace Sims1WidescreenPatcher.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = Locator.Current.GetService<IMainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
