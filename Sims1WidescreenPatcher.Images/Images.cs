using Serilog;
using Sims.Far;
using Sims1WidescreenPatcher.Images.Models;

namespace Sims1WidescreenPatcher.Images;

public static class Images
{
    private static readonly string[] BlackBackground =
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

    private static readonly string[] BlueBackground =
    {
        @"Downtown\largeback.bmp",
        @"Magicland\largeback.bmp",
        @"Studiotown\largeback.bmp",
        @"Downtown\dlgframe_1024x768.bmp",
        @"Magicland\dlgframe_1024x768.bmp",
        @"Studiotown\dlgframe_1024x768.bmp",
    };

    private static readonly string[] ScaleList =
    {
        @"cpanel\Backgrounds\TallSubPanel.TGA"
    };

    public static void RemoveGraphics(string path)
    {
        Log.Information("Begin removing graphics");
        Log.Debug("Path {@Path}", path);
        var dir = Path.GetDirectoryName(path);
        var deletePath = $"{dir}/UIGraphics/cpanel/Backgrounds/PanelBack.bmp";
        Log.Debug("Sims directory {@Dir}", dir);
        Log.Debug("Delete {Delete}", deletePath);
        File.Delete(deletePath);
        
        foreach (var i in BlackBackground)
        {
            deletePath = $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}";
            Log.Debug("Delete: {Delete}", deletePath);
            File.Delete(deletePath);
        }

        foreach (var i in BlueBackground)
        {
            deletePath = $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}";
            Log.Debug("Delete: {Delete}", deletePath);
            File.Delete(deletePath);
        }

        foreach (var i in ScaleList)
        {
            deletePath = $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}";
            Log.Debug("Delete: {Delete}", deletePath);
            File.Delete(deletePath);
        }
        
        Log.Information("End remove graphics");
    }

    public static void ModifySimsUi(string path, int width, int height, IProgress<double> progress)
    {
        Log.Information("Begin scaling Sims UI files");
        
        var dir = Path.GetDirectoryName(path) ?? string.Empty;
        var uigraphicsFarPath = Path.Combine(dir, "UIGraphics/UIGraphics.far");
        var far = new Far(uigraphicsFarPath);
        var jobs = new List<BaseImageProcessingJob>();

        Log.Debug("Path {@Path}", path);
        Log.Debug("Width {@Width}", width);
        Log.Debug("Height {@Height}", height);
        Log.Debug("UIGraphics.far path {@UIGraphicsFarPath}", uigraphicsFarPath);
        
        #region Create Jobs

        Log.Information("Create jobs");
        
        if (far.Manifest.ManifestEntries.Any(x => x.Filename == @"cpanel\Backgrounds\PanelBack.bmp"))
        {
            jobs.Add(new ScalePanelbackJob(far.GetBytes(@"cpanel\Backgrounds\PanelBack.bmp"),
                $"{dir}/UIGraphics/cpanel/Backgrounds/PanelBack.bmp", width, 100));
        }
        jobs.AddRange(BlackBackground.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
            .Select(i => new CompositeImageJob("#000000", far.GetBytes(i),
                $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}", width, height)));
        jobs.AddRange(BlueBackground.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
            .Select(i => new CompositeImageJob("#000052", far.GetBytes(i),
                $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}", width, height)));
        jobs.AddRange(ScaleList.Where(i => far.Manifest.ManifestEntries.Any(m => m.Filename == i))
            .Select(i => new ScaleTallSubPanelJob(far.GetBytes(i),
                $"{dir}/UIGraphics/{i.Replace('\\', Path.DirectorySeparatorChar)}", width, 150)));

        Log.Information("End create jobs");
        
        # endregion

        var totalJobs = jobs.Count;
        var current = 0;
        var lockObject = new object();
        
        Log.Debug("Total jobs {@TotalJobs}", totalJobs);

        #region Run Jobs

        Log.Information("Begin run jobs");
        
        Parallel.ForEach(jobs, job =>
        {
            Log.Debug("Begin job: {@Job}", job);
            job.Run();
            lock (lockObject)
            {
                current++;
                var calc = current / (double) totalJobs * 100;
                progress.Report(calc);
            }
        });

        Log.Information("End run jobs");
        
        #endregion
        
        Log.Information("End modify Sims UI");
    }
}