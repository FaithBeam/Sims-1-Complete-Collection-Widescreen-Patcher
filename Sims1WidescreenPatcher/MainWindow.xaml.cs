using ImageMagick;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Windows;

namespace HexEditApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sims|Sims.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                fileDialog.Text = openFileDialog.FileName;
            CheckForBackup(fileDialog.Text);
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (ForceCheckBox.IsChecked != true)
            {
                if (!CheckHash(fileDialog.Text, "42F9A3E11BD1A03515C77777CB97B5BC"))
                {
                    MessageBox.Show("MD5's do not match, canceling.");
                    return;
                }
            }
            BackupFile(fileDialog.Text);
            if (dgVoodoo2Checkbox.IsChecked == true)
                DownloadFiles(fileDialog.Text);
            EditFile(fileDialog.Text);
            CopyGraphics(fileDialog.Text);
            UninstallButton.IsEnabled = true;
            MessageBox.Show("Patched!");
        }

        private void CheckForBackup(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (File.Exists($@"{directory}\Sims Backup.exe"))
                UninstallButton.IsEnabled = true;
            else
                UninstallButton.IsEnabled = false;
        }

        private bool CheckHash(string path, string expectedMd5Hash)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            string md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return md5Hash.Equals(expectedMd5Hash, StringComparison.InvariantCultureIgnoreCase);
        }

        private void BackupFile(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            if (!File.Exists($@"{directory}\{filename} Backup.exe"))
                File.Copy(path, $@"{directory}\{filename} Backup.exe");
        }

        private void DownloadFiles(string path)
        {
            string directory = Path.GetDirectoryName(path);
            List<string> urls = new List<string> {
                "http://dege.freeweb.hu/dgVoodoo2/D3DCompiler_43.zip",
                "http://dege.freeweb.hu/dgVoodoo2/D3DCompiler_47.zip",
                "http://dege.freeweb.hu/dgVoodoo2/dgVoodoo2_55_4.zip"
            };
            TryRemoveDgVoodoo(directory);
            using var client = new WebClient();
            foreach (var url in urls)
            {
                string fileName = Path.GetFileName(url);
                client.DownloadFile(url, $@"{directory}\{fileName}");

                ZipFile.ExtractToDirectory($@"{directory}\{fileName}", $@"{directory}\");

                File.Delete($@"{directory}\{fileName}");
            }

            new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory($@"{directory}\MS\", directory);
            Directory.Delete($@"{directory}\3Dfx", true);
            Directory.Delete($@"{directory}\Doc", true);
            Directory.Delete($@"{directory}\MS", true);

            string text = File.ReadAllText($@"{directory}\dgVoodoo.conf");
            text = text.Replace("dgVoodooWatermark                   = true", "dgVoodooWatermark                   = false");
            File.WriteAllText($@"{directory}\dgVoodoo.conf", text);
        }

        private void EditFile(string path)
        {
            byte[] width = BitConverter.GetBytes(int.Parse(WidthTextBox.Text));
            byte[] height = BitConverter.GetBytes(int.Parse(HeightTextBox.Text));

            int widthOffset = 1001563;
            int heightOffset = 1001570;
            var bytes = File.ReadAllBytes(path);

            bytes[widthOffset] = width[0];
            bytes[widthOffset + 1] = width[1];

            bytes[heightOffset] = height[0];
            bytes[heightOffset + 1] = height[1];

            File.WriteAllBytes(path, bytes);
        }

        // TODO: Resize graphics and copy to folder
        private void CopyGraphics(string path)
        {
            string directory = Path.GetDirectoryName(path);
            int width = int.Parse(WidthTextBox.Text);
            int height = int.Parse(HeightTextBox.Text);
            Directory.CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Downtown");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Studiotown");

            using (MagickImage image = new MagickImage(@"UIGraphics\CPanel\Backgrounds\PanelBack.bmp"))
            {
                MagickGeometry size = new MagickGeometry(width, 100);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.BitDepth(8);
                image.ColorType = ColorType.TrueColor;
                image.Write($@"{directory}\UIGraphics\CPanel\Backgrounds\PanelBack.bmp");
            }

            using (MagickImage image = new MagickImage(@"UIGraphics\Downtown\largeback.bmp"))
            {
                MagickGeometry size = new MagickGeometry(width, height);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.BitDepth(8);
                image.ColorType = ColorType.TrueColor;
                image.Write($@"{directory}\UIGraphics\Downtown\largeback.bmp");
            }

            using (MagickImage image = new MagickImage(@"UIGraphics\Studiotown\dlgframe_1024x768.bmp"))
            {
                MagickGeometry size = new MagickGeometry(width, height);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.BitDepth(8);
                image.ColorType = ColorType.TrueColor;
                image.Write($@"{directory}\UIGraphics\Studiotown\dlgframe_1024x768.bmp");
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            string directory = Path.GetDirectoryName(fileDialog.Text);
            File.Delete(fileDialog.Text);
            File.Move($@"{directory}\Sims Backup.exe", $@"{directory}\Sims.exe");
            TryRemoveDgVoodoo(directory);
            UninstallButton.IsEnabled = false;
            MessageBox.Show("Uninstalled.");
        }

        private void TryRemoveDgVoodoo(string directory)
        {
            File.Delete($@"{directory}\D3DCompiler_43.dll");
            File.Delete($@"{directory}\d3dcompiler_47.dll");
            File.Delete($@"{directory}\D3D8.dll");
            File.Delete($@"{directory}\D3D9.dll");
            File.Delete($@"{directory}\D3DImm.dll");
            File.Delete($@"{directory}\DDraw.dll");
            File.Delete($@"{directory}\dgVoodoo.conf");
            File.Delete($@"{directory}\dgVoodooCpl.exe");
            File.Delete($@"{directory}\QuickGuide.html");
            File.Delete($@"{directory}\dgVoodoo2_55_4.zip");
            File.Delete($@"{directory}\UIGraphics\CPanel\Backgrounds\PanelBack.bmp");
            File.Delete($@"{directory}\UIGraphics\Downtown\largeback.bmp");
            File.Delete($@"{directory}\UIGraphics\Studiotown\dlgframe_1024x768.bmp");
            if (Directory.Exists($@"{directory}\3Dfx"))
                Directory.Delete($@"{directory}\3Dfx", true);
            if (Directory.Exists($@"{directory}\Doc"))
                Directory.Delete($@"{directory}\Doc", true);
            if (Directory.Exists($@"{directory}\MS"))
                Directory.Delete($@"{directory}\MS", true);
        }
    }
}
