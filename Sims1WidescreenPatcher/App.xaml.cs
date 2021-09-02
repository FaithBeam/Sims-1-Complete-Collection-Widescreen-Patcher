using System.Reflection;
using System.Windows;
using Serilog;
using Sims1WidescreenPatcher.Views;

namespace Sims1WidescreenPatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("Sims1WidescreenPatcher.log")
            .CreateLogger();
            
            Log.Debug($"{Assembly.GetExecutingAssembly().GetName().Name}\t{Assembly.GetExecutingAssembly().GetName().Version}");

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
