using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Sims.Far;
using Sims1WidescreenPatcher.Far.Models;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far
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
            @"Downtown\dlgframe_1024x768.bmp",
            @"Magicland\dlgframe_1024x768.bmp",
            @"Studiotown\dlgframe_1024x768.bmp",
        };

        public void RemoveGraphics(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            Log.Debug("Remove installed graphics from {DirectoryName}", directoryName);
            FileHelper.DeleteFile($@"{directoryName}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in _blackBackground)
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
            foreach (var i in _blueBackground)
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
        }

        public void CopyGraphics(string path, int width, int height, IProgress<double> progress)
        {
            var directory = Path.GetDirectoryName(path);
            var farLocation = Path.Combine(directory, @"UIGraphics\UIGraphics.far");
            var far = new Sims.Far.Far(farLocation);
            var jobs = new List<IJob>();

            #region Create Jobs

            if (far.Manifest.ManifestEntries.Any(m => m.Filename == @"cpanel\Backgrounds\PanelBack.bmp"))
                jobs.Add(new ScaleImageJob()
                {
                    Bytes = far.GetBytes(@"cpanel\Backgrounds\PanelBack.bmp"),
                    Output = $@"{directory}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp", Height = 100, Width = width
                });
            jobs.AddRange(_blackBackground.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
                .Select(i => new CompositeImageJob()
                {
                    Background = "blackbackground.png",
                    Bytes = far.GetBytes(i),
                    Output = $@"{directory}\UIGraphics\{i}",
                    Height = height,
                    Width = width
                }));
            jobs.AddRange(_blueBackground.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
                .Select(i => new CompositeImageJob()
                {
                    Background = "bluebackground.png",
                    Bytes = far.GetBytes(i),
                    Output = $@"{directory}\UIGraphics\{i}",
                    Height = height,
                    Width = width
                }));

            #endregion

            var totalImages = jobs.Count;
            var current = 0;
            var lockObject = new object();
            #region Run Jobs

            Parallel.ForEach(jobs, job =>
            {
                job.Extract();
                lock (lockObject)
                {
                    current++;
                    var calc = current / (double) totalImages * 100;
                    progress.Report(calc);
                }
            });
            
            #endregion
        }
    }
}