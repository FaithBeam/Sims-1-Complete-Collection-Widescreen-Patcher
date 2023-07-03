using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection
{
	public static class Bootstrapper
	{
		public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
		{
			ServicesBootstrapper.RegisterServices(services, resolver);
			ViewModelBootstrapper.RegisterViewModels(services, resolver);
		}
	}
}
