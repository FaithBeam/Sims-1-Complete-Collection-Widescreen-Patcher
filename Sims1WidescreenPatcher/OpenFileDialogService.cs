using Microsoft.Win32;

namespace Sims1WidescreenPatcher
{
    public class OpenFileDialogService
    {
        private const string EXECUTABLE = "Sims";
        public string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = $"{EXECUTABLE}|{EXECUTABLE}.exe|All files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };
            if ((bool)openFileDialog.ShowDialog())
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}
