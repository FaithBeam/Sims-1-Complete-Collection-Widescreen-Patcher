using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Serilog;
using Sims1WidescreenPatcher.UI.WPF.ViewModels;

namespace Sims1WidescreenPatcher.UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Regex _rx = new Regex(@"[\dx]", RegexOptions.Compiled);
        
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            DataContext = viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Log.CloseAndFlush();
        }

        private void ResolutionComboBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_rx.IsMatch(e.Text);
        }
    }
}
