using System.Windows;
using Sims1WidescreenPatcher.ViewModels;

namespace SimsWidescreenPatcher
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
            viewModel.Width = 1920;
            viewModel.Height = 1080;
        }
    }
}
