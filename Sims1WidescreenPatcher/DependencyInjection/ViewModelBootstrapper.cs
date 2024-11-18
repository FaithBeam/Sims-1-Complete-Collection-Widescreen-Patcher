using Microsoft.Extensions.DependencyInjection;
using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.DependencyInjection;

public static class ViewModelBootstrapper
{
    public static void RegisterViewModels(IServiceCollection services)
    {
        services
            .AddScoped<ICustomResolutionDialogViewModel, CustomResolutionDialogViewModel>()
            .AddScoped<CustomYesNoDialogViewModel>()
            .AddScoped<INotificationViewModel, NotificationViewModel>()
            .AddScoped<IMainWindowViewModel, MainWindowViewModel>()
            .AddScoped<IMainTabViewModel, MainTabViewModel>()
            .AddScoped<IExtrasTabViewModel, ExtrasTabViewModel>()
            .AddScoped<ICareerEditorTabViewModel, CareerEditorDialogViewModel>();
    }
}
