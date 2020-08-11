using log4net;
using System.Configuration;
using System.Windows;

namespace HexEditApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            if (ConfigurationManager.AppSettings["EnableLogging"] == "true")
            {
                log4net.Config.XmlConfigurator.Configure();
                log.Info("=============  Started Logging  =============");
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                log.Info(fvi.FileVersion);
            }
            else
            {
                LogManager.GetRepository().ResetConfiguration();
            }
            base.OnStartup(e);
        }
    }
}
