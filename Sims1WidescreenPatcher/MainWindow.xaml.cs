using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using log4net;
using Microsoft.Win32;
using PatternFinder;
using Sims.Far;

namespace Sims1WidescreenPatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> images = new List<string> {
            @"Community\Bus_loadscreen_1024x768.bmp",
            @"Downtown\Taxi_loadscreen_1024x768.bmp",
            @"Magicland\magicland_loadscreen_1024x768.bmp",
            @"Magicland\magicland_loadscreen_hole_1024x768.bmp",
            @"Nbhd\Bus_loadscreen_1024x768.bmp",
            @"Other\setup.bmp",
            @"Studiotown\Studiotown_loadscreen_1024x768.bmp",
            @"Studiotown\Studiotown_loadscreen_fan_1024x768.bmp",
            @"VIsland\vacation_loadscreen_1024x768.bmp",
            @"VIsland\vacation_loadscreen2_1024x768.bmp",
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
        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        private readonly string exeName = ConfigurationManager.AppSettings["Executable"];

        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Clicked browse button.");
            var openFileDialog = new OpenFileDialog
            {
                Filter = $"{exeName}|{exeName}.exe|All files (*.*)|*.*",
                RestoreDirectory = true // Necessary to fix bug on Windows XP where openfiledialog will change the program's current working directory
            };
            if (openFileDialog.ShowDialog() != true) return;
            FileDialog.Text = openFileDialog.FileName;
            log.Info($"Selected {FileDialog.Text}");
            CheckForBackup(FileDialog.Text);
        }

        private void PatchButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Clicked patch button.");
            if (!string.IsNullOrWhiteSpace(FileDialog.Text))
            {
                if (File.Exists(FileDialog.Text))
                {
                    log.Info("Before patch md5 is: " + GetMd5(FileDialog.Text));
                    log.Info("Resolution chosen is: " + WidthTextBox.Text + "x" + HeightTextBox.Text);
                    if (EditFile(FileDialog.Text))
                    {
                        log.Info("After patch md5 is: " + GetMd5(FileDialog.Text));
                        if (ConfigurationManager.AppSettings["ResizeUiElements"] == "true")
                        {
                            ExtractUigraphics(FileDialog.Text);
                            CopyGraphics(FileDialog.Text);
                            DeleteDirectory(@"Content\UIGraphics");
                        }
                        else
                        {
                            log.Info("Resize UI elements checkbox is not checked, not resizing or copying graphics.");
                        }
                        UninstallButton.IsEnabled = true;
                        var width = $"{int.Parse(WidthTextBox.Text):X4}";
                        width = width.Substring(2) + width.Substring(0, 2);
                        var height = $"{int.Parse(HeightTextBox.Text):X4}";
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
                    log.Info("File " + FileDialog.Text + " doesn't exist.");
                    MessageBox.Show("File " + FileDialog.Text + " doesn't exist.");
                }
            }
            else
            {
                log.Info("No file was selected.");
                MessageBox.Show($"Please select your {exeName}.exe.");
            }
        }

        private string GetMd5(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void CheckForBackup(string path)
        {
            var directory = Path.GetDirectoryName(path);
            UninstallButton.IsEnabled = File.Exists($@"{directory}\{exeName} Backup.exe");
        }

        private void BackupFile(string path)
        {
            log.Info("Creating backup.");
            var filename = Path.GetFileNameWithoutExtension(path);
            var directory = Path.GetDirectoryName(path);
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

        private bool EditFile(string path)
        {
            log.Info($"Hex editing {path}");
            var widthPattern = ConfigurationManager.AppSettings["WidthPattern"];
            var betweenPattern = ConfigurationManager.AppSettings["BetweenPattern"];
            var heightPattern = ConfigurationManager.AppSettings["HeightPattern"];
            var width = BitConverter.GetBytes(int.Parse(WidthTextBox.Text));
            var height = BitConverter.GetBytes(int.Parse(HeightTextBox.Text));
            var pattern = Pattern.Transform(widthPattern + " " + betweenPattern + " " + heightPattern);
            var bytes = File.ReadAllBytes(path);

            if (Pattern.Find(bytes, pattern, out var foundOffset))
            {
                BackupFile(FileDialog.Text);
                log.Info(widthPattern + " " + betweenPattern + " " + heightPattern + " found at " + foundOffset);
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                bytes[foundOffset + 2 + betweenPattern.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + betweenPattern.Trim().Split().Length + 1] = height[1];

                File.SetAttributes(path, FileAttributes.Normal);
                File.WriteAllBytes(path, bytes);
                return true;
            }

            return false;
        }

        private void CopyGraphics(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var width = int.Parse(WidthTextBox.Text);
            var height = int.Parse(HeightTextBox.Text);

            CreateDirectory($@"{directory}\UIGraphics\Community");
            CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            CreateDirectory($@"{directory}\UIGraphics\Magicland");
            CreateDirectory($@"{directory}\UIGraphics\Nbhd");
            CreateDirectory($@"{directory}\UIGraphics\Other");
            CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            CreateDirectory($@"{directory}\UIGraphics\Visland");
            CreateDirectory($@"{directory}\UIGraphics\Downtown");

            ScalePanelBack(@"Content\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", width, 100);
            foreach (var i in images)
            {
                if (!File.Exists($@"Content\UIGraphics\{i}"))
                {
                    log.Info($@"Couldn't find Content\UIGraphics\{i}");
                    continue;
                }
                CompositeImage("#000000", $@"Content\UIGraphics\{i}", $@"{directory}\UIGraphics\{i}", width, height);
            }
            if (File.Exists(@"Content\UIGraphics\Downtown\largeback.bmp"))
            {
                foreach (var i in largeBackLocations)
                    CompositeImage(@"#000052", @"Content\UIGraphics\Downtown\largeback.bmp", $@"{directory}\{i}", width, height);
            }
            else
            {
                log.Info(@"Content\UIGraphics\Downtown\largeback.bmp doesn't exist, skipping this section");
            }
            if (File.Exists(@"Content\UIGraphics\StudioTown\dlgframe_1024x768.bmp"))
            {
                foreach (var i in dlgFrameLocations)
                    CompositeImage(@"#000052", @"Content\UIGraphics\StudioTown\dlgframe_1024x768.bmp", $@"{directory}\{i}", width, height);
            }
            else
            {
                log.Info(@"Content\UIGraphics\StudioTown\dlgframe_1024x768.bmp doesn't exist, skipping this section");
            }
        }

        private void ExtractUigraphics(string pathToSimsExe)
        {
            var simsInstallationDirectory = Path.GetDirectoryName(pathToSimsExe);
            var uigraphicsPath = simsInstallationDirectory + @"\UIGraphics\UIGraphics.far";
            log.Info($"Extracting images from {uigraphicsPath}");
            if (!File.Exists(uigraphicsPath))
            {
                MessageBox.Show($"Couldn't find UIGraphics.far at {uigraphicsPath}");
                log.Info($"Couldn't find UIGraphics.far at {uigraphicsPath}");
            }
            CreateDirectory(@"Content\UIGraphics");
            var far = new Far(uigraphicsPath);
            var extractImages = new List<string>(images)
            {
                @"Downtown\largeback.bmp",
                @"Studiotown\dlgframe_1024x768.bmp",
                @"cpanel\Backgrounds\PanelBack.bmp"
            };
            far.Extract(outputDirectory: @"Content\UIGraphics", filter: extractImages);
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

        private void CompositeImage(string hexColorCode, string input, string output, int width, int height)
        {
            var sb = new StringBuilder($"convert.exe -size {width}x{height} canvas:\"{hexColorCode}\" \"{input}\" " +
                                       $"-gravity center -composite -depth 8 -type palette \"BMP3:{output}\"");
            log.Info(sb.ToString());
            var psi = new ProcessStartInfo("cmd", "/c" + sb)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
            }
        }

        private void ScalePanelBack(string input, string output, int width, int height)
        {
            var left = new ImageSize { Width = 286, Height = 100, X = 0, Y = 0 };
            var middle = new ImageSize { Width = 500, Height = 100, X = 286, Y = 0 };
            var right = new ImageSize { Width = 18, Height = 100, X = left.Width + middle.Width, Y = 0 };
            var sb = new StringBuilder();
            sb.Append($"convert.exe \"{input}\" +repage -write mpr:img +delete " +
                           $"( mpr:img -crop {left} ) +append " +
                           $"( mpr:img -crop {middle} -resize {width - left.Width - right.Width}x100! -geometry +{middle.X} ) +append " +
                           $"( mpr:img -crop {right} -geometry +{width - right.Width} ) +append " +
                           $"-depth 8 -type palette \"BMP3:{output}\"");
            log.Info(sb.ToString());
            var psi = new ProcessStartInfo("cmd", "/c" + sb)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            log.Info("Uninstall button pressed.");
            ConfigurationManager.RefreshSection("appSettings");
            var directory = Path.GetDirectoryName(FileDialog.Text);
            File.SetAttributes(FileDialog.Text, FileAttributes.Normal);
            File.Delete(FileDialog.Text);
            File.Move($@"{directory}\{exeName} Backup.exe", $@"{directory}\{exeName}.exe");
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
            DeleteFile($@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in images)
                DeleteFile($@"{directory}\UIGraphics\{i}");
            foreach (var i in largeBackLocations)
                DeleteFile($@"{directory}\{i}");
            foreach (var i in dlgFrameLocations)
                DeleteFile($@"{directory}\{i}");
        }

        private struct ImageSize
        {
            public int Width;
            public int Height;
            public int X;
            public int Y;

            public override string ToString()
            {
                return $"{Width}x{Height}+{X}+{Y}";
            }
        }
    }
}
