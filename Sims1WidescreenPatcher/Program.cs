using System.Reflection;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Serilog;
using Serilog.Formatting.Compact;
using Sims1WidescreenPatcher.DependencyInjection;
using Sims1WidescreenPatcher.UI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace Sims1WidescreenPatcher;

internal static class Program
{
    private static IServiceProvider? Container { get; set; }

    [STAThread]
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(new CompactJsonFormatter(), "Sims1WidescreenPatcherLog.clef")
            .MinimumLevel.Debug()
            .CreateLogger();
        var informationalVersion = (
            (AssemblyInformationalVersionAttribute)
                Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                    .FirstOrDefault()! ?? throw new InvalidOperationException()
        ).InformationalVersion;
        var name = Assembly.GetExecutingAssembly().GetName().Name;
        var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        Log.Information("{@Name}", name);
        Log.Information("{@Version}", informationalVersion);
        Log.Information("{@OSInformation}", osNameAndVersion);
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.UseMicrosoftDependencyResolver();
                var resolver = Locator.CurrentMutable;
                resolver.InitializeSplat();
                resolver.InitializeReactiveUI();
                Bootstrapper.Register(services);

                services.AddSingleton<
                    IActivationForViewFetcher,
                    AvaloniaActivationForViewFetcher
                >();
                services.AddSingleton<IPropertyBindingHook, AutoDataTemplateBindingHook>();
            })
            .Build();

        Container = host.Services;
        Container.UseMicrosoftDependencyResolver();

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal("{@Exception}", e);
            Log.CloseAndFlush();
            throw;
        }
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}
