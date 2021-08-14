using System.Windows;
using Serilog;
using Sims1WidescreenPatcher.ViewModels;

namespace Sims1WidescreenPatcher.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            DataContext = viewModel;
        }

        private void HeightTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int _);
        }

        private void WidthTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int _);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Log.CloseAndFlush();
        }
    }
}
