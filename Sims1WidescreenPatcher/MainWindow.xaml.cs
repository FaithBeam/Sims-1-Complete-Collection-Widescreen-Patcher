using ImageMagick;
using Microsoft.Win32;
using PatternFinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using log4net;
using System.Security.Cryptography;
using System.Configuration;

namespace HexEditApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string exeName = ConfigurationManager.AppSettings["Executable"];

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            InitializeComponent();
            string screenWidth = SystemParameters.PrimaryScreenWidth.ToString();
            string screenHeight = SystemParameters.PrimaryScreenHeight.ToString();
            WidthTextBox.Text = screenWidth;
            HeightTextBox.Text = screenHeight;
            log.Info($"Screen resolution detected as {screenWidth}x{screenHeight}");
            if (ConfigurationManager.AppSettings["PowerUser"] == "true")
            {
                widthPattern.IsEnabled = true;
                betweenPattern.IsEnabled = true;
                heightPattern.IsEnabled = true;
                resizeUiElementsCheckbox.IsEnabled = true;
            }
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
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
            openFileDialog.Filter = $"{this.exeName}|{this.exeName}.exe|All files (*.*)|*.*";
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
                if (File.Exists(fileDialog.Text))
                {
                    log.Info("Before patch md5 is: " + GetMd5(fileDialog.Text));
                    BackupFile(fileDialog.Text);
                    if (dgVoodoo2Checkbox.IsChecked == true)
                        ExtractVoodooZips(fileDialog.Text);
                    if (EditFile(fileDialog.Text))
                    {
                        if (resizeUiElementsCheckbox.IsChecked == true)
                            CopyGraphics(fileDialog.Text);
                        else
                            log.Info("Resize UI elements checkbox is not checked, not resizing or copying graphics.");
                        UninstallButton.IsEnabled = true;
                        var width = $"{int.Parse(WidthTextBox.Text):X4}";
                        var height = $"{int.Parse(HeightTextBox.Text):X4}";
                        widthPattern.Text = width.Substring(2) + " " + width.Substring(0, 2);
                        heightPattern.Text = height.Substring(2) + " " + height.Substring(0, 2);
                        log.Info("After patch md5 is: " + GetMd5(fileDialog.Text));
                        log.Info("Patched");
                        MessageBox.Show("Patched!");
                    }
                    else
                    {
                        log.Info($"Failed to find pattern: " + widthPattern.Text + " " + betweenPattern.Text + " " + heightPattern.Text);
                        MessageBox.Show("Failed to find pattern...");
                    }
                }
                else
                {
                    log.Info("File " + fileDialog.Text + " doesn't exist.");
                    MessageBox.Show("File " + fileDialog.Text + " doesn't exist.");
                }
            }
            else
            {
                log.Info("No file was selected.");
                MessageBox.Show($"Please select your {this.exeName}.exe.");
            }
        }

        private string GetMd5(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private void CheckForBackup(string path)
        {
            string directory = Path.GetDirectoryName(path);
            UninstallButton.IsEnabled = File.Exists($@"{directory}\{this.exeName} Backup.exe");
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
            {
                log.Info("A previous backup already exists, not creating another.");
            }
        }

        private void ExtractVoodooZips(string path)
        {
            string directory = Path.GetDirectoryName(path);
            var voodooZips = new List<string>
            {
                "D3DCompiler_43.zip",
                "D3DCompiler_47.zip",
                "dgVoodoo2_55_4.zip"
            };
            TryRemoveDgVoodoo(directory);
            foreach (var voodooZip in voodooZips)
            {
                string fileName = Path.GetFileName(voodooZip);
                log.Info($"Extracting {fileName}");
                ZipFile.ExtractToDirectory(voodooZip, $@"{directory}\");
            }

            log.Info("Deleting unneeded directories.");
            new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory($@"{directory}\MS\", directory);
            DeleteDirectory($@"{directory}\3Dfx");
            DeleteDirectory($@"{directory}\Doc");
            DeleteDirectory($@"{directory}\MS");

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
                log.Info(widthPattern.Text + " " + betweenPattern.Text + " " + heightPattern.Text + " found.");
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                bytes[foundOffset + 2 + betweenPattern.Text.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + betweenPattern.Text.Trim().Split().Length + 1] = height[1];

                File.WriteAllBytes(path, bytes);
                return true;
            }

            return false;
        }

        private void CopyGraphics(string path)
        {
            string directory = Path.GetDirectoryName(path);
            int width = int.Parse(WidthTextBox.Text);
            int height = int.Parse(HeightTextBox.Text);

            CreateDirectory($@"{directory}\UIGraphics\Community");
            CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            CreateDirectory($@"{directory}\UIGraphics\Magicland");
            CreateDirectory($@"{directory}\UIGraphics\Nbhd");
            CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            CreateDirectory($@"{directory}\UIGraphics\Visland");
            CreateDirectory($@"{directory}\UIGraphics\Downtown");

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

        private void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                log.Info(path + " already exists.");
            }
            else
            {
                log.Info("Creating directory: " + path);
                Directory.CreateDirectory(path);
            }
        }

        private void ScaleImageRle(string input, string output, int width, int height)
        {
            log.Info($"Resizing {input} to {output}");
            using MagickImage image = new MagickImage(input);
            MagickGeometry size = new MagickGeometry(width, height);
            size.IgnoreAspectRatio = true;
            image.Resize(size);
            image.Depth = 8;
            image.Settings.Format = MagickFormat.Bmp3;
            image.ColorType = ColorType.Palette;
            image.Alpha(AlphaOption.Off);
            image.Write(output);
        }

        private void ScaleImageRgb(string input, string output, int width, int height)
        {
            log.Info($"Resizing {input} to {output}");
            using MagickImage image = new MagickImage(input);
            MagickGeometry size = new MagickGeometry(width, height);
            size.IgnoreAspectRatio = true;
            image.Resize(size);
            image.Depth = 8;
            image.Settings.Format = MagickFormat.Bmp3;
            image.ColorType = ColorType.TrueColor;
            image.Write(output);
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Uninstall button pressed.");
            string directory = Path.GetDirectoryName(fileDialog.Text);
            File.Delete(fileDialog.Text);
            File.Move($@"{directory}\{this.exeName} Backup.exe", $@"{directory}\{this.exeName}.exe");
            TryRemoveDgVoodoo(directory);
            UninstallButton.IsEnabled = false;
            widthPattern.Text = "20 03";
            betweenPattern.Text = "00 00 C7 45 E0";
            heightPattern.Text = "58 02";
            MessageBox.Show("Uninstalled.");
        }

        private void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                log.Info($"Deleting {path}");
                File.Delete(path);
            }
            else
            {
                log.Info(path + " doesn't exist.");
            }
        }

        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                log.Info($"Deleting {path}");
                Directory.Delete(path, true);
            }
            else
            {
                log.Info(path + " doesn't exist.");
            }
        }

        private void TryRemoveDgVoodoo(string directory)
        {
            log.Info("Deleting previous installation.");
            DeleteFile($@"{directory}\D3DCompiler_43.dll");
            DeleteFile($@"{directory}\d3dcompiler_47.dll");
            DeleteFile($@"{directory}\D3D8.dll");
            DeleteFile($@"{directory}\D3D9.dll");
            DeleteFile($@"{directory}\D3DImm.dll");
            DeleteFile($@"{directory}\DDraw.dll");
            DeleteFile($@"{directory}\dgVoodoo.conf");
            DeleteFile($@"{directory}\dgVoodooCpl.exe");
            DeleteFile($@"{directory}\QuickGuide.html");
            DeleteFile($@"{directory}\UIGraphics\Community\Bus_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\CPanel\Backgrounds\PanelBack.bmp");
            DeleteFile($@"{directory}\UIGraphics\Downtown\largeback.bmp");
            DeleteFile($@"{directory}\UIGraphics\Downtown\dlgframe_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Downtown\Taxi_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Magicland\dlgframe_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Magicland\largeback.bmp");
            DeleteFile($@"{directory}\UIGraphics\Magicland\magicland_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Magicland\magicland_loadscreen_hole_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Nbhd\Bus_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Studiotown\largeback.bmp");
            DeleteFile($@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Studiotown\Studiotown_loadscreen_fan_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Visland\vacation_loadscreen_1024x768.bmp");
            DeleteFile($@"{directory}\UIGraphics\Visland\vacation_loadscreen2_1024x768.bmp");
            DeleteDirectory($@"{directory}\3Dfx");
            DeleteDirectory($@"{directory}\Doc");
            DeleteDirectory($@"{directory}\MS");
        }

        private void HeightTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.Parse(HeightTextBox.Text) > 1080)
                dgVoodoo2Checkbox.IsChecked = true;
        }
    }
}
