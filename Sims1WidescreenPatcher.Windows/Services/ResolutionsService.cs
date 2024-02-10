using System.Collections.Generic;
using System.Linq;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Windows.Services
{
	public class WindowsResolutionsService : IResolutionsService
	{
		public IEnumerable<Resolution> GetResolutions()
		{
			Log.Information("Begin enumerate resolutions");
			var resolutions = new List<Resolution>();
			var devMode = new DEVMODEW();
			var i = (ENUM_DISPLAY_SETTINGS_MODE)0;
			while (PInvoke.EnumDisplaySettings(null, i, ref devMode))
			{
				var newRes = new Resolution(devMode.dmPelsWidth, devMode.dmPelsHeight);
				if (resolutions.All(x => !x.Equals(newRes)) && newRes.Width >= 800 && newRes.Height >= 600)
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