using Sims1WidescreenPatcher.Core.Factories;
using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class FactoryBootstrapper
{
    public static void RegisterFactories(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.Register(() => new CheckboxViewModelFactory());
    }
}