﻿using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IImagesService
{
    void Install();
    void Uninstall();
}

public class ImagesService : IImagesService
{
    private string? _uiGraphicsPath;

    private readonly string[] _blackBackground =
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

    private readonly string[] _blueBackground =
    {
        @"Downtown\largeback.bmp",
        @"Magicland\largeback.bmp",
        @"Studiotown\largeback.bmp",
        @"Downtown\dlgframe_1024x768.bmp",
        @"Magicland\dlgframe_1024x768.bmp",
        @"Studiotown\dlgframe_1024x768.bmp",
    };

    private readonly IAppState _appState;
    private readonly IProgressService _progressService;
    private readonly IFar _far;

    public ImagesService(IAppState appState, IProgressService progressService, IFar far)
    {
        _appState = appState;
        _progressService = progressService;
        _far = far;
    }

    private const string TallSubPanel = @"cpanel\Backgrounds\TallSubPanel.TGA";

    private const string PanelBack = @"cpanel\Backgrounds\PanelBack.bmp";

    public void Install()
    {
        _uiGraphicsPath = GetUiGraphicsFolder();
        _far.PathToFar = CombineWithUiGraphicsPath("UIGraphics.far");
        var jobs = GetJobs();

        var totalJobs = jobs.Count;
        var current = 0;
        var lockObject = new object();

        Parallel.ForEach(
            jobs,
            job =>
            {
                job.Run();
                lock (lockObject)
                {
                    current++;
                    var calc = current / (double)totalJobs * 100;
                    _progressService.UpdateProgress(
                        calc,
                        $"{current}/{totalJobs}",
                        $"Scaling {job.BaseImageName}"
                    );
                }
            }
        );
    }

    public void Uninstall()
    {
        _uiGraphicsPath = GetUiGraphicsFolder();

        foreach (
            var i in _blackBackground
                .Concat(_blueBackground)
                .Concat(new List<string> { TallSubPanel, PanelBack }.AsReadOnly())
        )
        {
            DeleteUiGraphicsFile(i);
        }
    }

    private List<BaseImageProcessingJob> GetJobs()
    {
        _far.ParseFar();
        var jobs = new List<BaseImageProcessingJob>();

        if (_appState.Resolution is null)
        {
            return jobs;
        }

        if (_far.TryGetBytes(PanelBack, out var bytes))
        {
            jobs.Add(
                new ScalePanelBackJob
                {
                    ImageBytes = bytes,
                    Output = CombineWithUiGraphicsPath(PanelBack),
                    Width = _appState.Resolution.Width,
                    Height = 100,
                }
            );
        }

        jobs.AddRange(GetCompositeJobs(_blackBackground, "#000000"));

        jobs.AddRange(GetCompositeJobs(_blueBackground, "#000052"));

        if (_far.TryGetBytes(TallSubPanel, out bytes))
        {
            jobs.Add(
                new ScaleTallSubPanelJob
                {
                    ImageBytes = bytes,
                    Output = CombineWithUiGraphicsPath(TallSubPanel),
                    Width = _appState.Resolution.Width,
                    Height = 150,
                }
            );
        }

        return jobs;
    }

    private IEnumerable<BaseImageProcessingJob> GetCompositeJobs(
        IEnumerable<string> images,
        string color
    )
    {
        if (_appState.Resolution is null)
        {
            throw new Exception("Resolution is null");
        }
        foreach (var i in images)
        {
            if (_far.TryGetBytes(i, out var bytes))
            {
                yield return new CompositeImageJob
                {
                    Color = color,
                    Height = _appState.Resolution.Height,
                    Output = CombineWithUiGraphicsPath(i),
                    Width = _appState.Resolution.Width,
                    ImageBytes = bytes,
                };
            }
        }
    }

    private string GetUiGraphicsFolder()
    {
        if (string.IsNullOrWhiteSpace(_appState.SimsExePath))
        {
            throw new Exception("SimsExePath is null or empty");
        }
        var simsInstallDir = Directory.GetParent(_appState.SimsExePath)?.ToString();
        if (string.IsNullOrWhiteSpace(simsInstallDir))
        {
            throw new Exception("SimsExePath is null or empty");
        }
        return Path.Combine(simsInstallDir, "UIGraphics");
    }

    private void DeleteUiGraphicsFile(string path)
    {
        if (string.IsNullOrWhiteSpace(_uiGraphicsPath) || string.IsNullOrWhiteSpace(path))
            return;
        var combined = CombineWithUiGraphicsPath(path);
        if (File.Exists(combined))
        {
            File.Delete(combined);
        }
    }

    private string CombineWithUiGraphicsPath(string image)
    {
        if (string.IsNullOrWhiteSpace(_uiGraphicsPath))
        {
            throw new Exception("UIGraphics path is null");
        }

        return NormalizeString(Path.Combine(_uiGraphicsPath, image));
    }

    private static string NormalizeString(string path)
    {
        return path.Replace('\\', Path.DirectorySeparatorChar);
    }
}
