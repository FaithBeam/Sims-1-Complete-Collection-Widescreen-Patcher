using Microsoft.Extensions.DependencyInjection;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class StateBootstrapper
{
    public static void RegisterState(IServiceCollection services)
    {
        services.AddSingleton<IAppState, AppState>();
    }
}
