using System.Linq;
using System.Reflection;
using System.Windows;
using Serilog;
using Sims1WidescreenPatcher.UI.WPF.Services;
using Sims1WidescreenPatcher.UI.WPF.Views;

namespace Sims1WidescreenPatcher.UI.WPF
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
            var informationalVersion = ((AssemblyInformationalVersionAttribute) Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).FirstOrDefault())?.InformationalVersion;
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            Log.Debug("{Name}\t{InformationalVersion}", name, informationalVersion);

            var mainWindow = new MainWindow(new DialogService(), new OpenFileDialogService());
            mainWindow.Show();
        }
    }
}
