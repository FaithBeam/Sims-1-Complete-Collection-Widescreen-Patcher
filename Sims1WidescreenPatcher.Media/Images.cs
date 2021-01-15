using ImageMagick;
using Sims.Far;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sims1WidescreenPatcher.Media
{
    public class Images
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
        private readonly PatchOptions _options;
        private readonly int _totalImages;
        private int _current;

        public event Action<ScaleStatus> StatusChanged;
        public event Action Completed;
        public event Action<string> Failed;

        public Images(PatchOptions options)
        {
            _options = options;
            _totalImages = images.Count + largeBackLocations.Count + dlgFrameLocations.Count + 1;
            _current = 0;
        }

        public void ExtractUigraphics()
        {
            var simsInstallationDirectory = Path.GetDirectoryName(_options.Path);
            var uigraphicsPath = simsInstallationDirectory + @"\UIGraphics\UIGraphics.far";
            DirectoryHelper.CreateDirectory(@"Content\UIGraphics");
            var far = new Far(uigraphicsPath);
            var extractImages = new List<string>(images)
            {
                @"Downtown\largeback.bmp",
                @"Studiotown\dlgframe_1024x768.bmp",
                @"cpanel\Backgrounds\PanelBack.bmp"
            };
            far.Extract(outputDirectory: @"Content\UIGraphics", filter: extractImages);
        }

        public void RemoveGraphics()
        {
            var directory = Path.GetDirectoryName(_options.Path);
            foreach (var i in images)
                FileHelper.DeleteFile($@"{directory}\UIGraphics\{i}");
            foreach (var i in largeBackLocations)
                FileHelper.DeleteFile($@"{directory}\{i}");
            foreach (var i in dlgFrameLocations)
                FileHelper.DeleteFile($@"{directory}\{i}");
        }

        public void CopyGraphics()
        {
            string directory = Path.GetDirectoryName(_options.Path);

            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Community");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Magicland");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Nbhd");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Other");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Visland");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Downtown");

            ScaleImage(@"Content\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", _options.Width, 100);
            foreach (var i in images)
            {
                if (!File.Exists($@"Content\UIGraphics\{i}"))
                    continue;
                CompositeImage(@"Content\blackbackground.png", $@"Content\UIGraphics\{i}", $@"{directory}\UIGraphics\{i}", _options.Width, _options.Height);
            }
            if (File.Exists(@"Content\UIGraphics\Downtown\largeback.bmp"))
            {
                foreach (var i in largeBackLocations)
                    CompositeImage(@"Content\bluebackground.png", @"Content\UIGraphics\Downtown\largeback.bmp", $@"{directory}\{i}", _options.Width, _options.Height);
            }
            if (File.Exists(@"Content\UIGraphics\StudioTown\dlgframe_1024x768.bmp"))
            {
                foreach (var i in dlgFrameLocations)
                    CompositeImage(@"Content\bluebackground.png", @"Content\UIGraphics\StudioTown\dlgframe_1024x768.bmp", $@"{directory}\{i}", _options.Width, _options.Height);
            }

            // Cleanup
            DirectoryHelper.DeleteDirectory(@"Content\UIGraphics");
        }

        private void CompositeImage(string background, string overlay, string output, int width, int height)
        {
            _current++;
            double calc = (double)_current / (double)_totalImages * 100;
            _options.Progress.Report(calc);
            using (var compositeImage = new MagickImage(overlay))
            {
                using (var baseImage = new MagickImage(background))
                {
                    var size = new MagickGeometry(width, height)
                    {
                        IgnoreAspectRatio = true
                    };
                    baseImage.Resize(size);
                    baseImage.Composite(compositeImage, Gravity.Center);
                    baseImage.Depth = 8;
                    baseImage.Settings.Compression = CompressionMethod.RLE;
                    baseImage.Settings.Format = MagickFormat.Bmp3;
                    baseImage.ColorType = ColorType.Palette;
                    baseImage.Alpha(AlphaOption.Off);
                    baseImage.Write(output);
                }
            }
        }

        private void ScaleImage(string input, string output, int width, int height)
        {
            _current++;
            double calc = (double)_current / (double)_totalImages * 100;
            _options.Progress.Report(calc);
            using (var image = new MagickImage(input))
            {
                var size = new MagickGeometry(width, height)
                {
                    IgnoreAspectRatio = true
                };
                image.Resize(size);
                image.Depth = 8;
                image.Settings.Compression = CompressionMethod.RLE;
                image.Settings.Format = MagickFormat.Bmp3;
                image.ColorType = ColorType.Palette;
                image.Alpha(AlphaOption.Off);
                image.Write(output);
            }
        }
    }
}
