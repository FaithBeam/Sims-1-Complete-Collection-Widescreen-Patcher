using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Linux.Services;
using Sims1WidescreenPatcher.MacOS.Services;
using Sims1WidescreenPatcher.Windows.Services;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using FindSimsPathService = Sims1WidescreenPatcher.Windows.Services.FindSimsPathService;

namespace Sims1WidescreenPatcher.DependencyInjection
{
    public static class ServicesBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            RegisterCommonServices(services);
            RegisterPlatformSpecificServices(services);
        }

        private static void RegisterPlatformSpecificServices(IServiceCollection services)
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(5))
            {
                services.AddScoped<IResolutionsService, WindowsResolutionsService>()
                    .AddScoped<IFindSimsPathService, FindSimsPathService>();
            }
            else if (OperatingSystem.IsMacOS())
            {
                services.AddScoped<IResolutionsService, MacOsResolutionService>()
                    .AddScoped<IFindSimsPathService, MacOS.Services.FindSimsPathService>();
            }
            else if (OperatingSystem.IsLinux())
            {
                services.AddScoped<IResolutionsService, LinuxResolutionService>()
                    .AddScoped<IFindSimsPathService, Linux.Services.FindSimsPathService>();
            }
            else
            {
                throw new InvalidOperationException("Unknown platform");
            }
        }

        private static void RegisterCommonServices(IServiceCollection services)
        {
            services.AddScoped<IFar, Far>()
                .AddScoped<IPatchFileService, PatchFileService>()
                .AddScoped<IProgressService, ProgressService>()
                .AddScoped<ICheatsService, CheatsService>()
                .AddScoped<IResolutionPatchService, ResolutionPatchService>()
                .AddScoped<IWrapperService, WrapperService>()
                .AddScoped<IUninstallService, UninstallService>()
                .AddScoped<IImagesService, ImagesService>();
        }
    }
}