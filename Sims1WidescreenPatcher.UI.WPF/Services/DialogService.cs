using System.Windows;

namespace Sims1WidescreenPatcher.UI.WPF.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
