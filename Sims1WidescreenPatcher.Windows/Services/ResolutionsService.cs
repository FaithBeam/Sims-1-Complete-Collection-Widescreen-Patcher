using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Windows.Services
{
	[SupportedOSPlatform("windows5.0")]
	public class WindowsResolutionsService : IResolutionsService
	{
		public IEnumerable<Resolution> GetResolutions()
		{
			Log.Information("Begin enumerate resolutions");
			var resolutions = new HashSet<Resolution>();
			var vDevMode = new DEVMODEW();
			var i = 0;
			while (PInvoke.EnumDisplaySettings(null, (ENUM_DISPLAY_SETTINGS_MODE)i, ref vDevMode))
			{
				var newRes = new Resolution((int)vDevMode.dmPelsWidth, (int)vDevMode.dmPelsHeight);
				if (newRes.Width >= 800 && newRes.Height >= 600)
				{
					resolutions.Add(newRes);
				}

				i++;
			}

			Log.Debug("Resolutions {@Resolutions}", resolutions);
			Log.Information("End enumerate resolutions");
			return resolutions.OrderBy(x => x.Height * x.Width);
		}
	}
}