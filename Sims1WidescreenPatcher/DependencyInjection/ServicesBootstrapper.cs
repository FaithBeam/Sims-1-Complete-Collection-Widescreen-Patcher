using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Linux.Services;
using Sims1WidescreenPatcher.MacOS.Services;
using Sims1WidescreenPatcher.Windows.Services;
using System.Runtime.InteropServices;
using Autofac;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using FindSimsPathService = Sims1WidescreenPatcher.Windows.Services.FindSimsPathService;

namespace Sims1WidescreenPatcher.DependencyInjection
{
    public static class ServicesBootstrapper
    {
        public static void RegisterServices(ContainerBuilder services)
        {
            RegisterCommonServices(services);
            RegisterPlatformSpecificServices(services);
        }

        private static void RegisterPlatformSpecificServices(ContainerBuilder services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                services.RegisterType<WindowsResolutionsService>().As<IResolutionsService>().SingleInstance();
                services.RegisterType<FindSimsPathService>().As<IFindSimsPathService>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                services.RegisterType<MacOsResolutionService>().As<IResolutionsService>().SingleInstance();
                services.RegisterType<MacOS.Services.FindSimsPathService>().As<IFindSimsPathService>();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.RegisterType<LinuxResolutionService>().As<IResolutionsService>().SingleInstance();
                services.RegisterType<Linux.Services.FindSimsPathService>().As<IFindSimsPathService>();
            }
            else
            {
                throw new InvalidOperationException("Unknown platform");
            }
        }

        private static void RegisterCommonServices(ContainerBuilder services)
        {
            services.RegisterType<Far>().As<IFar>();
            services.RegisterType<PatchFileService>().As<IPatchFileService>();
            services.RegisterType<ProgressService>().As<IProgressService>().SingleInstance();
            services.RegisterType<CheatsService>().As<ICheatsService>();
            services.RegisterType<ResolutionPatchService>().As<IResolutionPatchService>();
            services.RegisterType<UninstallService>().As<IUninstallService>();
            services.RegisterType<ImagesService>().As<IImagesService>();
        }
    }
}
