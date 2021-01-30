using ImageMagick;
using Serilog;
using Sims.Far;
using Sims1WidescreenPatcher.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
        private readonly int _totalImages;
        private int _current;
        private readonly Far _far;
        private readonly string _path;
        private readonly int _width;
        private readonly int _height;
        private readonly IProgress<double> _progress;
        private readonly string _uigraphicsPath;

        public Images(string path, int width, int height, Progress<double> progress)
        {
            _path = path;
            _width = width;
            _height = height;
            _progress = progress;
            _totalImages = images.Count + largeBackLocations.Count + dlgFrameLocations.Count + 1;
            _current = 0;
            _uigraphicsPath = Path.GetDirectoryName(_path) + @"\UIGraphics\UIGraphics.far";
            _far = new Far(_uigraphicsPath);
            _far.ParseFar();
        }

        public void RemoveGraphics()
        {
            Log.Debug($"Remove installed graphics from {Path.GetDirectoryName(_path)}.");
            var directory = Path.GetDirectoryName(_path);
            FileHelper.DeleteFile($@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in images)
                FileHelper.DeleteFile($@"{directory}\UIGraphics\{i}");
            foreach (var i in largeBackLocations)
                FileHelper.DeleteFile($@"{directory}\{i}");
            foreach (var i in dlgFrameLocations)
                FileHelper.DeleteFile($@"{directory}\{i}");
        }

        private byte[] GetBytes(string name)
        {
            Log.Debug($"Get bytes of {name} from {_uigraphicsPath}.");
            var me = _far.Manifest.ManifestEntries.Where(f => string.Equals(f.Filename, name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            _far.FarStream.Seek(me.FileOffset, SeekOrigin.Begin);
            var bytes = new byte[me.FileLength1];
            _far.FarStream.Read(bytes, 0, me.FileLength1);
            return bytes;
        }

        public void CopyGraphics()
        {
            string directory = Path.GetDirectoryName(_path);

            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Community");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\CPanel\Backgrounds");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Magicland");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Nbhd");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Other");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Studiotown");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Visland");
            DirectoryHelper.CreateDirectory($@"{directory}\UIGraphics\Downtown");

            ScaleImage(GetBytes(@"cpanel\Backgrounds\PanelBack.bmp"), $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", _width, 100);
            foreach (var i in images)
                CompositeImage("blackbackground.png", GetBytes(i), $@"{directory}\UIGraphics\{i}", _width, _height);
            foreach (var i in largeBackLocations)
                CompositeImage("bluebackground.png", GetBytes(@"Downtown\largeback.bmp"), $@"{directory}\{i}", _width, _height);
            foreach (var i in dlgFrameLocations)
                CompositeImage("bluebackground.png", GetBytes(@"StudioTown\dlgframe_1024x768.bmp"), $@"{directory}\{i}", _width, _height);
            _far.FarStream.Close();
        }

        private void CompositeImage(string background, byte[] overlay, string output, int width, int height)
        {
            Log.Debug($"Composite {background} with bytes to {output}. Width: {width}, Height: {height}.");
            _current++;
            double calc = (double)_current / (double)_totalImages * 100;
            _progress.Report(calc);
            using (var compositeImage = new MagickImage(overlay))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Sims1WidescreenPatcher.Media.Resources.{background}"))
                using (var baseImage = new MagickImage(stream))
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

        private void ScaleImage(byte[] bytes, string output, int width, int height)
        {
            Log.Debug($"Scale bytes to {output}. Width: {width}, Height: {height}.");
            _current++;
            double calc = (double)_current / (double)_totalImages * 100;
            _progress.Report(calc);
            using (var image = new MagickImage(bytes))
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
