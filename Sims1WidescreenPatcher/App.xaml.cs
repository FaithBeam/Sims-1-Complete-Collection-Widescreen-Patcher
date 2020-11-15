using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Windows;

namespace SimsWidescreenPatcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            if (ConfigurationManager.AppSettings["EnableLogging"] == "true")
            {
                log4net.Config.XmlConfigurator.Configure();
                log.Info("=============  Started Logging  =============");
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                log.Info(fvi.FileVersion);
                log.Info(Environment.OSVersion);
                log.Info($"Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
                log.Info($"Current directory: {Directory.GetCurrentDirectory()}");
            }
            else
            {
                LogManager.GetRepository().ResetConfiguration();
            }
            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            log.Error(errorMessage);
            log.Error(e.Exception.StackTrace);
            log.Error(e.Exception.InnerException);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
