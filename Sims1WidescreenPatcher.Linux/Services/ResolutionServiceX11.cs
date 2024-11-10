using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Linux.Services
{
    public class ResolutionServiceX11 : IResolutionsService
    {
        public IEnumerable<Resolution> GetResolutions()
        {
            Log.Information("Begin enumerate resolutions");
            HashSet<Resolution> resolutions;

            var display = X11.XOpenDisplay(IntPtr.Zero);
            if (display == IntPtr.Zero)
            {
                throw new Exception("Failed to open display");
            }

            try
            {
                var root = X11.XDefaultRootWindow(display);
                resolutions = EnumerateResolutions(display, root);
            }
            finally
            {
                X11.XCloseDisplay(display);
            }

            Log.Debug("Resolutions {@Resolutions}", resolutions);
            Log.Information("End enumerate resolutions");
            return resolutions;
        }

        private static HashSet<Resolution> EnumerateResolutions(IntPtr display, IntPtr root)
        {
            var screenResourcesPtr = X11.XRRGetScreenResources(display, root);
            if (screenResourcesPtr == IntPtr.Zero)
            {
                throw new Exception("Failed to get screen resources");
            }

            var screenResources = Marshal.PtrToStructure<X11.XrrScreenResources>(
                screenResourcesPtr
            );
            var sizeXrrModeInfo = Marshal.SizeOf<X11.XrrModeInfo>();
            var modesArr = new X11.XrrModeInfo[screenResources.nmode];
            var curPtr = screenResources.modes;
            for (var i = 0; i < screenResources.nmode; i++)
            {
                modesArr[i] = Marshal.PtrToStructure<X11.XrrModeInfo>(curPtr + sizeXrrModeInfo * i);
            }

            X11.XRRFreeScreenResources(screenResourcesPtr);
            return modesArr
                .Where(x => x is { width: >= 800, height: >= 600 })
                .Select(x => new Resolution((int)x.width, (int)x.height))
                .OrderBy(x => x.Height * x.Width)
                .ToHashSet();
        }
    }
}

internal static class X11
{
    [DllImport("libX11.so.6")]
    internal static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport("libX11.so.6")]
    internal static extern void XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6")]
    internal static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libXrandr.so.2")]
    internal static extern IntPtr XRRGetScreenResources(IntPtr display, IntPtr root);

    [DllImport("libXrandr.so.2")]
    internal static extern IntPtr XRRFreeScreenResources(IntPtr resources);

    [StructLayout(LayoutKind.Sequential)]
    internal struct XrrScreenResources
    {
        public ulong timestamp;
        public ulong configTimestamp;
        public int ncrtc;
        public IntPtr crtcs;
        public int noutput;
        public IntPtr outputs;
        public int nmode;
        public IntPtr modes;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XrrModeInfo
    {
        public ulong id;
        public uint width;
        public uint height;
        public ulong dotClock;
        public uint hSyncStart;
        public uint hSyncEnd;
        public uint hTotal;
        public uint hSkew;
        public uint vSyncStart;
        public uint vSyncEnd;
        public uint vTotal;
        public IntPtr name;
        public uint nameLen;
        public ulong modeFlags;
    }
}
