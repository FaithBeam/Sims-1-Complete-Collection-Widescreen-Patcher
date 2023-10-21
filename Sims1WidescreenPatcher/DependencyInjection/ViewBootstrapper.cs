using Autofac;
using Sims1WidescreenPatcher.UI.Tabs;
using Sims1WidescreenPatcher.UI.Views;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class ViewBootstrapper
{
    public static void RegisterViews(ContainerBuilder services)
    {
        services.RegisterType<MainWindow>();
        services.RegisterType<MainTab>();
        services.RegisterType<OptionalTab>();
    }
}