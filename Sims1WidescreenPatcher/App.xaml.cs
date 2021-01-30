using Serilog;
using SimsWidescreenPatcher.Views;
using System.Windows;

namespace SimsWidescreenPatcher
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

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
