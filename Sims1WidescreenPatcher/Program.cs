using System.Reflection;
using Autofac;
using Avalonia;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Serilog;
using Serilog.Formatting.Compact;
using Sims1WidescreenPatcher.DependencyInjection;
using Sims1WidescreenPatcher.UI;
using Splat.Autofac;

namespace Sims1WidescreenPatcher;

internal static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.File(new CompactJsonFormatter(), "Sims1WidescreenPatcherLog.clef")
			.MinimumLevel.Debug()
			.CreateLogger();
		var informationalVersion = ((AssemblyInformationalVersionAttribute)Assembly
				.GetExecutingAssembly()
				.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
				.FirstOrDefault()! ?? throw new InvalidOperationException())
			.InformationalVersion;
		var name = Assembly.GetExecutingAssembly().GetName().Name;
		var osNameAndVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
		Log.Information("{@Name}", name);
		Log.Information("{@Version}", informationalVersion);
		Log.Information("{@OSInformation}", osNameAndVersion);
		var builder = new ContainerBuilder();
		Bootstrapper.Register(builder);
		builder.RegisterType<AvaloniaActivationForViewFetcher>().As<IActivationForViewFetcher>().SingleInstance();
		var autofacResolver = builder.UseAutofacDependencyResolver();
		builder.RegisterInstance(autofacResolver);
		autofacResolver.InitializeReactiveUI();
		var container = builder.Build();
		autofacResolver.SetLifetimeScope(container);
		RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
		
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
			.LogToTrace();
}