using ImageMagick;
using Microsoft.Win32;
using PatternFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;
using log4net;

namespace HexEditApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            InitializeComponent();
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            log.Error(errorMessage);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info($"Clicked browse button.");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Sims|Sims.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                fileDialog.Text = openFileDialog.FileName;
                log.Info($"Selected {fileDialog.Text}");
                CheckForBackup(fileDialog.Text);
            }
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Clicked patch button.");
            if (!string.IsNullOrWhiteSpace(fileDialog.Text))
            {
                BackupFile(fileDialog.Text);
                if (dgVoodoo2Checkbox.IsChecked == true)
                    DownloadFiles(fileDialog.Text);
                if (EditFile(fileDialog.Text))
                {
                    CopyGraphics(fileDialog.Text);
                    UninstallButton.IsEnabled = true;
                    var width = $"{int.Parse(WidthTextBox.Text):X4}";
                    var height = $"{int.Parse(HeightTextBox.Text):X4}";
                    widthPattern.Text = width.Substring(2) + " " + width.Substring(0, 2);
                    heightPattern.Text = height.Substring(2) + " " + height.Substring(0, 2);
                    log.Info("Patched");
                    MessageBox.Show("Patched!");
                }
                else
                {
                    log.Info($"Failed to find pattern");
                    MessageBox.Show("Failed to find pattern...");
                }
            }
            else
            {
                log.Info("No file was selected.");
                MessageBox.Show("Please select your sims.exe.");
            }
        }

        private void CheckForBackup(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (File.Exists($@"{directory}\Sims Backup.exe"))
                UninstallButton.IsEnabled = true;
            else
                UninstallButton.IsEnabled = false;
        }

        private void BackupFile(string path)
        {
            log.Info("Creating backup.");
            string filename = Path.GetFileNameWithoutExtension(path);
            string directory = Path.GetDirectoryName(path);
            if (!File.Exists($@"{directory}\{filename} Backup.exe"))
            {
                File.Copy(path, $@"{directory}\{filename} Backup.exe");
                log.Info($@"Created backup {directory}\{filename} Backup.exe");
            }
            else
                log.Info("A previous backup already exists, not creating another.");
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
                log.Info($"Downloading {fileName}");
                client.DownloadFile(url, $@"{directory}\{fileName}");

                log.Info($"Extracting {fileName}");
                ZipFile.ExtractToDirectory($@"{directory}\{fileName}", $@"{directory}\");

                log.Info($"Deleting {fileName}");
                File.Delete($@"{directory}\{fileName}");
            }

            log.Info("Deleting unneeded directories.");
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory($@"{directory}\MS\", directory);
            Directory.Delete($@"{directory}\3Dfx", true);
            Directory.Delete($@"{directory}\Doc", true);
            Directory.Delete($@"{directory}\MS", true);

            log.Info("Editing dgvoodoo.conf.");
            string text = File.ReadAllText($@"{directory}\dgVoodoo.conf");
            text = text.Replace("dgVoodooWatermark                   = true", "dgVoodooWatermark                   = false");
            text = text.Replace("FastVideoMemoryAccess               = false", "FastVideoMemoryAccess               = true");
            File.WriteAllText($@"{directory}\dgVoodoo.conf", text);
        }

        private bool EditFile(string path)
        {
            log.Debug($"Hex editing {path}");
            byte[] width = BitConverter.GetBytes(int.Parse(WidthTextBox.Text));
            byte[] height = BitConverter.GetBytes(int.Parse(HeightTextBox.Text));

            var bytes = File.ReadAllBytes(path);
            var pattern = Pattern.Transform(widthPattern.Text + " " + betweenPattern.Text + " " + heightPattern.Text);

            if (Pattern.Find(bytes, pattern, out long foundOffset))
            {
                log.Info($"{pattern} found.");
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                bytes[foundOffset + 2 + betweenPattern.Text.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + betweenPattern.Text.Trim().Split().Length + 1] = height[1];

                File.WriteAllBytes(path, bytes);
                return true;
            }

            log.Info($"{pattern} not found.");
            return false;
        }

        private void CopyGraphics(string path)
        {
            string directory = Path.GetDirectoryName(path);
            int width = int.Parse(WidthTextBox.Text);
            int height = int.Parse(HeightTextBox.Text);

            Directory.CreateDirectory($@"{directory}\UIGraphics\Community");
            Directory.CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Downtown");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Magicland");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Nbhd");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            Directory.CreateDirectory($@"{directory}\UIGraphics\Visland");

            ScaleImageRle(@"UIGraphics\Community\Bus_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Community\Bus_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\CPanel\Backgrounds\PanelBack.bmp", $@"{directory}\UIGraphics\CPanel\Backgrounds\PanelBack.bmp", width, 100);
            ScaleImageRgb(@"UIGraphics\Downtown\largeback.bmp", $@"{directory}\UIGraphics\Downtown\largeback.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Downtown\dlgframe_1024x768.bmp", $@"{directory}\UIGraphics\Downtown\dlgframe_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Downtown\Taxi_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Downtown\Taxi_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Magicland\dlgframe_1024x768.bmp", $@"{directory}\UIGraphics\Magicland\dlgframe_1024x768.bmp", width, height);
            ScaleImageRgb(@"UIGraphics\Magicland\largeback.bmp", $@"{directory}\UIGraphics\Magicland\largeback.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Magicland\magicland_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Magicland\magicland_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Magicland\magicland_loadscreen_hole_1024x768.bmp", $@"{directory}\UIGraphics\Magicland\magicland_loadscreen_hole_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Nbhd\Bus_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Nbhd\Bus_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Studiotown\dlgframe_1024x768.bmp", $@"{directory}\UIGraphics\Studiotown\dlgframe_1024x768.bmp", width, height);
            ScaleImageRgb(@"UIGraphics\Studiotown\largeback.bmp", $@"{directory}\UIGraphics\Studiotown\largeback.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Studiotown\Studiotown_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Studiotown\Studiotown_loadscreen_fan_1024x768.bmp", $@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_fan_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Visland\vacation_loadscreen_1024x768.bmp", $@"{directory}\UIGraphics\Visland\vacation_loadscreen_1024x768.bmp", width, height);
            ScaleImageRle(@"UIGraphics\Visland\vacation_loadscreen2_1024x768.bmp", $@"{directory}\UIGraphics\Visland\vacation_loadscreen2_1024x768.bmp", width, height);
        }

        private void ScaleImageRle(string input, string output, int width, int height)
        {
            log.Info($"Resizing {input} to {output}");
            using (MagickImage image = new MagickImage(input))
            {
                MagickGeometry size = new MagickGeometry(width, height);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.Depth = 8;
                image.Settings.Format = MagickFormat.Bmp3;
                image.ColorType = ColorType.Palette;
                image.Alpha(AlphaOption.Off);
                image.Write(output);
            }
        }

        private void ScaleImageRgb(string input, string output, int width, int height)
        {
            log.Info($"Resizing {input} to {output}");
            using (MagickImage image = new MagickImage(input))
            {
                MagickGeometry size = new MagickGeometry(width, height);
                size.IgnoreAspectRatio = true;
                image.Resize(size);
                image.Depth = 8;
                image.Settings.Format = MagickFormat.Bmp3;
                image.ColorType = ColorType.TrueColor;
                image.Write(output);
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Uninstall button pressed.");
            string directory = Path.GetDirectoryName(fileDialog.Text);
            File.Delete(fileDialog.Text);
            File.Move($@"{directory}\Sims Backup.exe", $@"{directory}\Sims.exe");
            TryRemoveDgVoodoo(directory);
            UninstallButton.IsEnabled = false;
            widthPattern.Text = "20 03";
            betweenPattern.Text = "00 00 C7 45 E0";
            heightPattern.Text = "58 02";
            MessageBox.Show("Uninstalled.");
        }

        private void TryRemoveDgVoodoo(string directory)
        {
            log.Info("Deleting previous installation.");
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
            File.Delete($@"{directory}\UIGraphics\Community\Bus_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\CPanel\Backgrounds\PanelBack.bmp");
            File.Delete($@"{directory}\UIGraphics\Downtown\largeback.bmp");
            File.Delete($@"{directory}\UIGraphics\Downtown\dlgframe_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Downtown\Taxi_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Magicland\dlgframe_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Magicland\largeback.bmp");
            File.Delete($@"{directory}\UIGraphics\Magicland\magicland_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Magicland\magicland_loadscreen_hole_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Nbhd\Bus_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Studiotown\dlgframe_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Studiotown\largeback.bmp");
            File.Delete($@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_fan_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Visland\vacation_loadscreen_1024x768.bmp");
            File.Delete($@"{directory}\UIGraphics\Visland\vacation_loadscreen2_1024x768.bmp");
            if (Directory.Exists($@"{directory}\3Dfx"))
                Directory.Delete($@"{directory}\3Dfx", true);
            if (Directory.Exists($@"{directory}\Doc"))
                Directory.Delete($@"{directory}\Doc", true);
            if (Directory.Exists($@"{directory}\MS"))
                Directory.Delete($@"{directory}\MS", true);
        }

        private void WidthPattern_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void BetweenPattern_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void HeightPattern_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
