using Autofac;
using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.DependencyInjection
{
    public static class ViewModelBootstrapper
    {
        public static void RegisterViewModels(ContainerBuilder services)
        {
            services.RegisterType<CustomResolutionDialogViewModel>().As<ICustomResolutionDialogViewModel>();
            services.RegisterType<CustomYesNoDialogViewModel>().AsSelf();
            services.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>();
            services.RegisterType<MainTabViewModel>().As<IMainTabViewModel>();
            services.RegisterType<OptionalTabViewModel>().As<IOptionalTabViewModel>();
        }
    }
}
