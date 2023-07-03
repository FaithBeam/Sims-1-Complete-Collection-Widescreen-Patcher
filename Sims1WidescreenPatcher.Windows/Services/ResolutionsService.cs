using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;

namespace Sims1WidescreenPatcher.Windows.Services
{
	public class WindowsResolutionsService : IResolutionsService
	{
		[DllImport("user32.dll")]
		private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

		[StructLayout(LayoutKind.Sequential)]
		private struct DEVMODE
		{
			private const int CCHDEVICENAME = 0x20;
			private const int CCHFORMNAME = 0x20;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			private string dmDeviceName;
			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;
			public int dmPositionX;
			public int dmPositionY;
			public int dmDisplayOrientation;
			public int dmDisplayFixedOutput;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string dmFormName;
			public short dmLogPixels;
			public int dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;
			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;
		}

		public IEnumerable<Resolution> GetResolutions()
		{
			Log.Information("Begin enumerate resolutions");
			var resolutions = new List<Resolution>();
			var vDevMode = new DEVMODE();
			var i = 0;
			while (EnumDisplaySettings(null, i, ref vDevMode))
			{
				var newRes = new Resolution(vDevMode.dmPelsWidth, vDevMode.dmPelsHeight);
				if (!resolutions.Contains(newRes, Resolution.WidthHeightComparer) && newRes.Width >= 800 && newRes.Height >= 600)
				{
					resolutions.Add(newRes);
				}
				i++;
			}
			Log.Debug("Resolutions {@Resolutions}", resolutions);
			Log.Information("End enumerate resolutions");
			return resolutions.OrderBy(x => x.Height * x.Width).ToList();
		}
	}
}