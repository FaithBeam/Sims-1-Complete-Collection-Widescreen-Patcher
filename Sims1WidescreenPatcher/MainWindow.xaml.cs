using ImageMagick;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using log4net;
using System.Security.Cryptography;
using System.Configuration;
using PatternFinder;
using Ionic.Zip;
using System.Threading.Tasks;

namespace HexEditApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> images = new List<string> {
            @"Content\UIGraphics\Community\Bus_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\Downtown\Taxi_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\Magicland\magicland_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\Magicland\magicland_loadscreen_hole_1024x768.bmp",
            @"Content\UIGraphics\Nbhd\Bus_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\Other\setup.bmp",
            @"Content\UIGraphics\Studiotown\Studiotown_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\Studiotown\Studiotown_loadscreen_fan_1024x768.bmp",
            @"Content\UIGraphics\VIsland\vacation_loadscreen_1024x768.bmp",
            @"Content\UIGraphics\VIsland\vacation_loadscreen2_1024x768.bmp",
        };
        private readonly List<string> largeBackLocations = new List<string>
        {
            @"UIGraphics\Downtown\largeback.bmp",
            @"UIGraphics\Magicland\largeback.bmp",
            @"UIGraphics\Studiotown\largeback.bmp",
        };
        private readonly List<string> dlgFrameLocations = new List<string>
        {
            @"UIGraphics\Downtown\dlgframe_1024x768.bmp",
            @"UIGraphics\Magicland\dlgframe_1024x768.bmp",
            @"UIGraphics\Studiotown\dlgframe_1024x768.bmp",
        };
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string exeName = ConfigurationManager.AppSettings["Executable"];

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
            if (ConfigurationManager.AppSettings["CheckInstallation"] == "true")
                CheckFiles();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Clicked browse button.");
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
                    log.Info("Resolution chosen is: " + WidthTextBox.Text + "x" + HeightTextBox.Text);
                    if (dgVoodoo2Checkbox.IsChecked == true)
                        ExtractVoodooZips(fileDialog.Text);
                    if (EditFile(fileDialog.Text))
                    {
                        log.Info("After patch md5 is: " + GetMd5(fileDialog.Text));
                        if (ConfigurationManager.AppSettings["ResizeUiElements"] == "true")
                            CopyGraphics(fileDialog.Text);
                        else
                            log.Info("Resize UI elements checkbox is not checked, not resizing or copying graphics.");
                        UninstallButton.IsEnabled = true;
                        string width = $"{int.Parse(WidthTextBox.Text):X4}";
                        width = width.Substring(2) + width.Substring(0, 2);
                        string height = $"{int.Parse(HeightTextBox.Text):X4}";
                        height = height.Substring(2) + height.Substring(0, 2);
                        ConfigurationManager.AppSettings["WidthPattern"] = width;
                        ConfigurationManager.AppSettings["HeightPattern"] = height;
                        log.Info("Patched");
                        MessageBox.Show("Patched!");
                    }
                    else
                    {
                        log.Info("Failed to find pattern: " + ConfigurationManager.AppSettings["WidthPattern"] + " " + ConfigurationManager.AppSettings["BetweenPattern"] + " " + ConfigurationManager.AppSettings["HeightPattern"]);
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
                File.SetAttributes($@"{directory}\{filename} Backup.exe", FileAttributes.Normal);
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
            TryRemoveDgVoodoo(directory);
            foreach (var zip in new string[] { @"Content\D3DCompiler_47.zip", @"Content\dgVoodoo2_64.zip" })
            {
                log.Info($"Extracting {zip}");
                ZipFile.Read(zip).ExtractAll($@"{directory}\");
            }

            log.Info("Deleting unneeded directories.");
            foreach (var file in Directory.GetFiles($@"{directory}\MS\x86"))
                File.Move(file, $@"{directory}\{Path.GetFileName(file)}");
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
            log.Info($"Hex editing {path}");
            string widthPattern = ConfigurationManager.AppSettings["WidthPattern"];
            string betweenPattern = ConfigurationManager.AppSettings["BetweenPattern"];
            string heightPattern = ConfigurationManager.AppSettings["HeightPattern"];
            byte[] width = BitConverter.GetBytes(int.Parse(WidthTextBox.Text));
            byte[] height = BitConverter.GetBytes(int.Parse(HeightTextBox.Text));
            Pattern.Byte[] pattern = Pattern.Transform(widthPattern + " " + betweenPattern + " " + heightPattern);
            byte[] bytes = File.ReadAllBytes(path);

            if (Pattern.Find(bytes, pattern, out long foundOffset))
            {
                BackupFile(fileDialog.Text);
                log.Info(widthPattern + " " + betweenPattern + " " + heightPattern + " found at " + foundOffset);
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                bytes[foundOffset + 2 + betweenPattern.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + betweenPattern.Trim().Split().Length + 1] = height[1];

                File.WriteAllBytes(path, bytes);
                return true;
            }

            return false;
        }

        private void CheckFiles()
        {
            log.Info("Checking the existance of local resources");
            if (!CheckFiles(@"Content\UIGraphics\cpanel\Backgrounds\PanelBack.bmp"))
                return;
            if (!CheckFiles(@"Content\UIGraphics\bluebackground.png"))
                return;
            foreach (var item in images)
                if (!CheckFiles(item))
                    return;
        }

        private bool CheckFiles(string path)
        {
            if (File.Exists(path))
            {
                log.Info($"{path} found.");
                return true;
            }
            else
            {
                log.Error($"Couldn't find {path}.");
                MessageBox.Show($"Couldn't find {path}. Check your installation.");
                PatchButton.IsEnabled = false;
                return false;
            }
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
            CreateDirectory($@"{directory}\UIGraphics\Other");
            CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            CreateDirectory($@"{directory}\UIGraphics\Visland");
            CreateDirectory($@"{directory}\UIGraphics\Downtown");

            ScaleImage(@"Content\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", width, 100);
            foreach (var i in images)
                CompositeImage(@"Content\UIGraphics\blackbackground.png", i, $@"{directory}\{i.Replace("Content\\", "")}", width, height);
            foreach (var i in largeBackLocations)
                CompositeImage(@"Content\UIGraphics\bluebackground.png", @"Content\UIGraphics\largeback.bmp", $@"{directory}\{i}", width, height);
            foreach (var i in dlgFrameLocations)
                CompositeImage(@"Content\UIGraphics\bluebackground.png", @"Content\UIGraphics\dlgframe_1024x768.bmp", $@"{directory}\{i}", width, height);
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

        private void CompositeImage(string background, string overlay, string output, int width, int height)
        {
            using var compositeImage = new MagickImage(overlay);
            using var baseImage = new MagickImage(background);
            log.Info($@"Compositing {overlay} over {background} to {output}");
            var size = new MagickGeometry(width, height);
            size.IgnoreAspectRatio = true;
            baseImage.Resize(size);
            baseImage.Composite(compositeImage, Gravity.Center);
            baseImage.Depth = 8;
            baseImage.Settings.Compression = ImageMagick.CompressionMethod.RLE;
            baseImage.Settings.Format = MagickFormat.Bmp3;
            baseImage.ColorType = ColorType.Palette;
            baseImage.Alpha(AlphaOption.Off);
            baseImage.Write(output);
        }

        private void ScaleImage(string input, string output, int width, int height)
        {
            using var image = new MagickImage(input);
            var size = new MagickGeometry(width, height);
            log.Info($"Resizing {input} to {output}");
            size.IgnoreAspectRatio = true;
            image.Resize(size);
            image.Depth = 8;
            image.Settings.Compression = ImageMagick.CompressionMethod.RLE;
            image.Settings.Format = MagickFormat.Bmp3;
            image.ColorType = ColorType.Palette;
            image.Alpha(AlphaOption.Off);
            image.Write(output);
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Uninstall button pressed.");
            ConfigurationManager.RefreshSection("appSettings");
            string directory = Path.GetDirectoryName(fileDialog.Text);
            File.SetAttributes(fileDialog.Text, FileAttributes.Normal);
            File.Delete(fileDialog.Text);
            File.Move($@"{directory}\{this.exeName} Backup.exe", $@"{directory}\{this.exeName}.exe");
            TryRemoveDgVoodoo(directory);
            UninstallButton.IsEnabled = false;
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
            DeleteFile($@"{directory}\d3dcompiler_47.dll");
            DeleteFile($@"{directory}\D3D8.dll");
            DeleteFile($@"{directory}\D3D9.dll");
            DeleteFile($@"{directory}\D3DImm.dll");
            DeleteFile($@"{directory}\DDraw.dll");
            DeleteFile($@"{directory}\dgVoodoo.conf");
            DeleteFile($@"{directory}\dgVoodooCpl.exe");
            DeleteFile($@"{directory}\QuickGuide.html");
            DeleteFile($@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in images)
                DeleteFile($@"{directory}\{i.Replace(@"Content\", "")}");
            foreach (var i in largeBackLocations)
                DeleteFile($@"{directory}\{i}");
            foreach (var i in dlgFrameLocations)
                DeleteFile($@"{directory}\{i}");
            DeleteDirectory($@"{directory}\3Dfx");
            DeleteDirectory($@"{directory}\Doc");
            DeleteDirectory($@"{directory}\MS");
        }

        private void HeightTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(HeightTextBox.Text, out int height))
                if (height > 1080)
                    dgVoodoo2Checkbox.IsChecked = true;
        }
    }
}
