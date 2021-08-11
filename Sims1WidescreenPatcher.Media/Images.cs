using Serilog;
using Sims.Far;
using Sims1WidescreenPatcher.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sims1WidescreenPatcher.Media.Models;

namespace Sims1WidescreenPatcher.Media
{
    public class Images
    {
        private readonly List<string> _blackBackground = new List<string>
        {
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

        private readonly List<string> _blueBackground = new List<string>
        {
            @"Downtown\largeback.bmp",
            @"Magicland\largeback.bmp",
            @"Studiotown\largeback.bmp",
            @"Downtown\dlgframe2.bmp",
            @"Magicland\dlgframe_1024x768.bmp",
            @"Studiotown\dlgframe_1024x768.bmp",
        };

        private int _totalImages;
        private int _current;
        private readonly Far _far;
        private readonly string _path;
        private readonly int _width;
        private readonly int _height;
        private readonly IProgress<double> _progress;
        private readonly List<IJob> _jobs = new List<IJob>();

        public Images(string path, int width, int height, IProgress<double> progress)
        {
            _path = path;
            _width = width;
            _height = height;
            _progress = progress;
            _current = 0;
            _far = new Far(Path.GetDirectoryName(_path) + @"\UIGraphics\UIGraphics.far");
        }

        public void RemoveGraphics()
        {
            Log.Debug($"Remove installed graphics from {Path.GetDirectoryName(_path)}.");
            var directory = Path.GetDirectoryName(_path);
            FileHelper.DeleteFile($@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in _blackBackground)
                FileHelper.DeleteFile($@"{directory}\UIGraphics\{i}");
            foreach (var i in _blueBackground)
                FileHelper.DeleteFile($@"{directory}\{i}");
        }

        public void CopyGraphics()
        {
            var directory = Path.GetDirectoryName(_path);

            #region Create Jobs

            if (_far.Manifest.ManifestEntries.Any(m => m.Filename == @"cpanel\Backgrounds\PanelBack.bmp"))
                _jobs.Add(new ScaleImageJob()
                {
                    Bytes = _far.GetBytes(@"cpanel\Backgrounds\PanelBack.bmp"),
                    Output = $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", Height = 100, Width = _width
                });
            foreach (var i in _blackBackground.Where(i => _far.Manifest.ManifestEntries.Any(m => m.Filename == i)))
                _jobs.Add(new CompositeImageJob()
                {
                    Background = "blackbackground.png", Bytes = _far.GetBytes(i),
                    Output = $@"{directory}\UIGraphics\{i}", Height = _height, Width = _width
                });
            foreach (var i in _blueBackground.Where(i => _far.Manifest.ManifestEntries.Any(m => m.Filename == i)))
                _jobs.Add(new CompositeImageJob()
                {
                    Background = "bluebackground.png", Bytes = _far.GetBytes(i),
                    Output = $@"{directory}\UIGraphics\{i}", Height = _height, Width = _width
                });

            #endregion

            _totalImages = _jobs.Count;

            #region Run Jobs

            foreach (var job in _jobs)
            {
                job.DoWork();
                _current++;
                var calc = _current / (double) _totalImages * 100;
                _progress.Report(calc);
            }

            #endregion
        }
    }
}