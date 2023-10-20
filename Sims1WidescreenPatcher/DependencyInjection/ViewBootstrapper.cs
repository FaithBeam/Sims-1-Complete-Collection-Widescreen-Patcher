using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.UI.Tabs;
using Sims1WidescreenPatcher.UI.Views;
using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class ViewBootstrapper
{
    public static void RegisterViews(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => 
                new MainWindow(resolver.GetService<IMainWindowViewModel>() 
                               ?? throw new InvalidOperationException()));
        services.RegisterLazySingleton(() =>
            new MainTab(resolver.GetService<IMainTabViewModel>() ?? throw new InvalidOperationException()));
        services.RegisterLazySingleton(() => new OptionalTab());
    }
}