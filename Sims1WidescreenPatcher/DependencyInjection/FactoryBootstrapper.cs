using Microsoft.Extensions.DependencyInjection;
using Sims1WidescreenPatcher.Core.Factories;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class FactoryBootstrapper
{
    public static void RegisterFactories(IServiceCollection services)
    {
        services
            .AddScoped<CheckboxViewModelFactory>()
            .AddScoped<ICareerEditorViewModelFactory, CareerEditorViewModelFactory>();
    }
}
