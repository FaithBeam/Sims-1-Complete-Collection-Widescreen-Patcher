using Autofac;
using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class Bootstrapper
{
	public static void Register(ContainerBuilder services)
	{
		ServicesBootstrapper.RegisterServices(services);
		FactoryBootstrapper.RegisterFactories(services);
		ViewModelBootstrapper.RegisterViewModels(services);
		ViewBootstrapper.RegisterViews(services);
	}
}