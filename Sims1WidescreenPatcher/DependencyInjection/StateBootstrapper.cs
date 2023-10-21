using Autofac;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class StateBootstrapper
{
    public static void RegisterState(ContainerBuilder services)
    {
        services.RegisterType<AppState>().As<IAppState>().SingleInstance();
    }
}