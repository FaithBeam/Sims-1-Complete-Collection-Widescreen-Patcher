using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.Utilities.Services;
using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection
{
    public static class ViewModelBootstrapper
    {
        public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.Register(() => new CustomResolutionDialogViewModel());
            services.Register(() => new CustomYesNoDialogViewModel());
            services.Register(() => new MainWindowViewModel(
                resolver.GetService<IResolutionsService>()!,
                resolver.GetService<CustomYesNoDialogViewModel>()!,
                resolver.GetService<CustomResolutionDialogViewModel>()!,
                resolver.GetService<IProgressService>()!,
                resolver.GetService<IFindSimsPathService>()!));
        }
    }
}
