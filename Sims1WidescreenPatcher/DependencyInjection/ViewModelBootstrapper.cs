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
            services.Register(() => new CustomResolutionDialogViewModel(), typeof(ICustomResolutionDialogViewModel));
            services.Register(() => new CustomYesNoDialogViewModel());
            services.Register(() => new MainWindowViewModel(
                resolver.GetService<IResolutionsService>() ?? throw new InvalidOperationException(),
                resolver.GetService<CustomYesNoDialogViewModel>() ?? throw new InvalidOperationException(),
                resolver.GetService<ICustomResolutionDialogViewModel>() ?? throw new InvalidOperationException(),
                resolver.GetService<IProgressService>() ?? throw new InvalidOperationException(),
                resolver.GetService<IFindSimsPathService>() ?? throw new InvalidOperationException()),
                typeof(IMainWindowViewModel));
        }
    }
}
