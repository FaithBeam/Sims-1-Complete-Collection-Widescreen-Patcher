using Autofac;
using Sims1WidescreenPatcher.Core.Factories;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class FactoryBootstrapper
{
    public static void RegisterFactories(ContainerBuilder services)
    {
        services.RegisterType<CheckboxViewModelFactory>().AsSelf();
    }
}