using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;

namespace Sims1WidescreenPatcher.Services;

public class MacOsResolutionService : IResolutionsService
{
    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern CgDirectDisplayId CGMainDisplayID();

    // Used this file as a reference for private CoreGraphics APIs https://github.com/robbertkl/ResolutionMenu/blob/master/Resolution%20Menu/DisplayModeMenuItem.m
    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern void CGSGetNumberOfDisplayModes(CgDirectDisplayId display, ref int nModes);
    
    [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
    private static extern void CGSGetDisplayModeDescriptionOfLength(CgDirectDisplayId display, int idx, ref CgsDisplayMode mode, int length);

    public IEnumerable<Resolution> GetResolutions()
    {
        var resolutions = new List<Resolution>();
        var nModes = 0;
        var id = CGMainDisplayID();
        CGSGetNumberOfDisplayModes(id, ref nModes);
        for (var i = 0; i < nModes; i++)
        {
            CgsDisplayMode mode = default;
            CGSGetDisplayModeDescriptionOfLength(id, i, ref mode, Marshal.SizeOf(mode));
            var resolution = new Resolution((int)mode.width, (int)mode.height);
            if (!resolutions.Contains(resolution, Resolution.WidthHeightComparer))
            {
                resolutions.Add(resolution);
            }
        }
        Log.Debug("Resolutions {@Resolutions}", resolutions);
        Log.Information("End enumerate resolutions");
        return resolutions;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    private struct CgsDisplayMode
    {
        private readonly uint modeNumber;
        private readonly uint flags;
        public readonly uint width;
        public readonly uint height;
        private readonly uint depth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 170)]
        private readonly byte[] unknown;
        private readonly ushort freq;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private readonly byte[] more_unknown;
        private readonly float density;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CgDirectDisplayId
    {
        private readonly uint id;
    }
}