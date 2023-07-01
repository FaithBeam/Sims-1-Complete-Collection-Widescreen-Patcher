using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Linux.Services;
using Sims1WidescreenPatcher.MacOS.Services;
using Sims1WidescreenPatcher.Utilities.Models;
using Sims1WidescreenPatcher.Windows.Services;
using Splat;
using System.Runtime.InteropServices;

namespace Sims1WidescreenPatcher.DependencyInjection
{
    public static class ServicesBootstrapper
    {
        public static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterCommonServices(services, resolver);
            RegisterPlatformSpecificServices(services, resolver);
        }

        private static void RegisterPlatformSpecificServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.RegisterLazySingleton<IResolutionsService>(() => new WindowsResolutionsService());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                services.RegisterLazySingleton<IResolutionsService>(() => new MacOsResolutionService());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.RegisterLazySingleton<IResolutionsService>(() => new LinuxResolutionService());
            }
            else
            {
                throw new InvalidOperationException("Unknown platform");
            }
        }

        private static void RegisterCommonServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton(() => new ProgressPct());
        }
    }
}
