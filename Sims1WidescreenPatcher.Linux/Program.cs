﻿using System;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using Serilog;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Services;
using Sims1WidescreenPatcher.UI;
using Splat;

namespace Sims1WidescreenPatcher
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Sims1WidescreenPatcher.log")
                .MinimumLevel.Debug()
                .CreateLogger();
            var informationalVersion = ((AssemblyInformationalVersionAttribute)Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                    .FirstOrDefault()! ?? throw new InvalidOperationException())
                .InformationalVersion;
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            Log.Information("{@Name}", name);
            Log.Information("{@Version}", informationalVersion);
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

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI()
                .AfterPlatformServicesSetup(_ => Locator.RegisterResolverCallbackChanged(() =>
                {
                    if (Locator.CurrentMutable is null)
                    {
                        return;
                    }

                    PlatformRegistrationManager.SetRegistrationNamespaces(RegistrationNamespace.Avalonia);
                    RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
                    Locator.CurrentMutable.RegisterConstant(new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
                    Locator.CurrentMutable.RegisterConstant(new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));
                    Locator.CurrentMutable.RegisterConstant(new LinuxResolutionService(), typeof(IResolutionsService));
                }));
    }
}