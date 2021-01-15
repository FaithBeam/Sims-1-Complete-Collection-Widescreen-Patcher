using System.Windows;

namespace Sims1WidescreenPatcher
{
    public class MessageBoxService
    {
        public MessageBoxResult ShowMessageBox(string message)
        {
            return MessageBox.Show(message, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
