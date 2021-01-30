using Sims1WidescreenPatcher.Services;
using System.Windows;

namespace Sims1WidescreenPatcher
{
    public class DialogService : IDialogService
    {
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message, "", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
