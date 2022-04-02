using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.Far.Models;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far
{
    public static class Images
    {
        private static readonly List<string> _blackBackground = new List<string>
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

        private static readonly List<string> _blueBackground = new List<string>
        {
            @"Downtown\largeback.bmp",
            @"Magicland\largeback.bmp",
            @"Studiotown\largeback.bmp",
            @"Downtown\dlgframe_1024x768.bmp",
            @"Magicland\dlgframe_1024x768.bmp",
            @"Studiotown\dlgframe_1024x768.bmp",
        };

        private static readonly List<string> _transparentBackground = new List<string>
        {
            @"cpanel\Catalog\BuyCFour.bmp",
            @"cpanel\Catalog\BuyCThree.bmp",
            @"cpanel\Catalog\BuyCTwo.bmp",
            @"cpanel\Catalog\BuyDOutdoor.bmp",
            @"cpanel\Catalog\BuyDShops.bmp",
            @"cpanel\Catalog\BuyDStreet.bmp",
            @"cpanel\Catalog\BuyMOutdoor.bmp",
            @"cpanel\Catalog\BuyMTmagic.bmp",
            @"cpanel\Catalog\BuySTDining.bmp",
            @"cpanel\Catalog\BuySTMisc.bmp",
            @"cpanel\Catalog\BuySTShops.bmp",
            @"cpanel\Catalog\BuySTSpa.bmp",
            @"cpanel\Catalog\BuySTStudio.bmp",
            @"cpanel\Catalog\BuyVFour.bmp",
            @"cpanel\Catalog\BuyVOne.bmp",
            @"cpanel\Catalog\BuyVThree.bmp",
            @"cpanel\Catalog\BuyVTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\BuySubSortAll.bmp",
            @"cpanel\Catalog\SubSortIcons\BuySubSortOther.bmp",
            @"cpanel\Catalog\SubSortIcons\Appliances\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Appliances\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Appliances\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Appliances\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityAppliances.BMP",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityDecorative.BMP",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityElectronics.bmp",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityLighting.BMP",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityMisc.bmp",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunityPlumbing.BMP",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunitySeating.BMP",
            @"cpanel\Catalog\SubSortIcons\CommunitySubsort\BuySubSortCommunitySurfaces.BMP",
            @"cpanel\Catalog\SubSortIcons\Decorative\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Decorative\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Decorative\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Decorative\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownAppliances.BMP",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownDecorative.BMP",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownElectronics.bmp",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownLighting.BMP",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownMisc.bmp",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownPlumbing.BMP",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownSeating.BMP",
            @"cpanel\Catalog\SubSortIcons\DowntownSubsort\BuySubSortDowntownSurfaces.BMP",
            @"cpanel\Catalog\SubSortIcons\Electronics\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Electronics\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Electronics\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Electronics\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\Lighting\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Lighting\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Lighting\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Lighting\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortMagic.BMP",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortPets.BMP",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Miscellaneous\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\Plumbing\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Plumbing\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Plumbing\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Plumbing\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomAppliances.BMP",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomDecorative.BMP",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomElectronics.bmp",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomLighting.BMP",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomMisc.bmp",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomPlumbing.BMP",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomSeating.BMP",
            @"cpanel\Catalog\SubSortIcons\RoomSubsort\BuySubSortRoomSurfaces.BMP",
            @"cpanel\Catalog\SubSortIcons\Seating\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Seating\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Seating\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Seating\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\Surfaces\BuySubSortFour.BMP",
            @"cpanel\Catalog\SubSortIcons\Surfaces\BuySubSortOne.BMP",
            @"cpanel\Catalog\SubSortIcons\Surfaces\BuySubSortThree.BMP",
            @"cpanel\Catalog\SubSortIcons\Surfaces\BuySubSortTwo.bmp",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationAppliances.BMP",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationDecorative.BMP",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationElectronics.bmp",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationLighting.BMP",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationMisc.bmp",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationPlumbing.BMP",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationSeating.BMP",
            @"cpanel\Catalog\SubSortIcons\VacationSubsort\BuySubSortVacationSurfaces.BMP"
        };

        private static readonly List<ReplaceColorJob> ReplaceColorJobs = new List<ReplaceColorJob>()
        {
            new ReplaceColorJob()
            {
                ImagePath = @"cpanel\Build\TerrainDownIcon.BMP",
                ReplaceColor = new MagickColor((byte) 0, (byte) 0, (byte) 82),
                Percentage = 1
            },
            new ReplaceColorJob()
            {
                ImagePath = @"cpanel\Build\TerrainLevelIcon.BMP",
                ReplaceColor = new MagickColor((byte) 0, (byte) 0, (byte) 82),
                Percentage = 1
            },
            new ReplaceColorJob()
            {
                ImagePath = @"cpanel\Build\TerrainUpIcon.BMP",
                ReplaceColor = new MagickColor((byte) 0, (byte) 0, (byte) 82),
                Percentage = 5
            },
        };

        public static void RemoveGraphics(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            Log.Debug("Remove installed graphics from {DirectoryName}", directoryName);
            FileHelper.DeleteFile($@"{directoryName}\UIGraphics\cpanel\Backgrounds\PanelBack.bmp");
            foreach (var i in _blackBackground)
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
            foreach (var i in _blueBackground)
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
            foreach (var i in _transparentBackground)
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
            foreach (var i in ReplaceColorJobs.Select(x => x.ImagePath))
                FileHelper.DeleteFile($@"{directoryName}\UIGraphics\{i}");
        }

        public static void CopyGraphics(string path, int width, int height, IProgress<double> progress)
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
            jobs.AddRange(_transparentBackground.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
                .Select(i => new TransparentBackgroundJob()
                {
                    Bytes = far.GetBytes(i),
                    Output = $@"{directory}\UIGraphics\{i}"
                }));
            foreach (var rcJob in ReplaceColorJobs.Where(rcJob => far.Manifest.ManifestEntries.Any(m => m.Filename == rcJob.ImagePath)))
            {
                rcJob.Bytes = far.GetBytes(rcJob.ImagePath);
                rcJob.Output = $@"{directory}\UIGraphics\{rcJob.ImagePath}";
                jobs.Add(rcJob);
            }

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