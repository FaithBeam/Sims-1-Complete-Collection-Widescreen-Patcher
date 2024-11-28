using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Linux.Services;

public class ResolutionServiceWayland : IResolutionsService
{
    private const string DrmDir = "/sys/class/drm/";

    public IEnumerable<Resolution> GetResolutions()
    {
        if (!Directory.Exists(DrmDir))
        {
            throw new DirectoryNotFoundException($"Could not find directory {DrmDir}");
        }

        HashSet<Resolution> resolutions = new();
        var modeFiles = Directory.EnumerateFiles(
            DrmDir,
            "modes",
            enumerationOptions: new EnumerationOptions
            {
                RecurseSubdirectories = true,
                MaxRecursionDepth = 1,
            }
        );
        foreach (var modeFile in modeFiles)
        {
            var modesTxt = File.ReadAllText(modeFile);
            if (string.IsNullOrWhiteSpace(modesTxt))
            {
                continue;
            }

            var lineSplit = modesTxt.Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries
            );
            foreach (var line in lineSplit)
            {
                var modeSplit = line.Split('x', StringSplitOptions.RemoveEmptyEntries);
                resolutions.Add(new Resolution(int.Parse(modeSplit[0]), int.Parse(modeSplit[1])));
            }
        }

        return resolutions
            .Where(x => x is { Width: >= 800, Height: >= 600 })
            .OrderBy(x => x.Height * x.Width);
    }
}
