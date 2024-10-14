using Microsoft.Extensions.DependencyInjection;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class Bootstrapper
{
	public static void Register(IServiceCollection services)
	{
		StateBootstrapper.RegisterState(services);
		ServicesBootstrapper.RegisterServices(services);
		FactoryBootstrapper.RegisterFactories(services);
		ViewModelBootstrapper.RegisterViewModels(services);
	}
}