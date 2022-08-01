using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Services;
using Sims1WidescreenPatcher.UI;
using Splat;

namespace Sims1WidescreenPatcher;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Sims1WidescreenPatcher.log")
            .MinimumLevel.Debug()
            .CreateLogger();
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal("{@Exception}", e);
            Log.CloseAndFlush();
            throw;
        }
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI().AfterPlatformServicesSetup(_ => Locator.RegisterResolverCallbackChanged(() =>
            {
                PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Avalonia);
                RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
                Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(),
                    typeof(IActivationForViewFetcher));
                Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(),
                    typeof(IPropertyBindingHook));
                Locator.CurrentMutable.RegisterConstant(new MacOsResolutionService(), typeof(IResolutionsService));
            }));
}