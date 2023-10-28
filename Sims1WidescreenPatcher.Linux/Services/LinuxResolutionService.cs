using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Linux.Services
{
	public class LinuxResolutionService : IResolutionsService
	{
		private const string FileName = "/bin/bash";
		private const string Cmd = "-c xrandr";
		private readonly Regex _resolutionRx = new Regex(@"\d{3,}x\d{3,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public IEnumerable<Resolution> GetResolutions()
		{
			Log.Information("Begin enumerate resolutions");
			var psi = new ProcessStartInfo()
			{
				FileName = FileName,
				Arguments = Cmd,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			using (var process = Process.Start(psi))
			{
				var output = "";
				if (process != null)
				{
					process.WaitForExit();
					output = process.StandardOutput.ReadToEnd();
				}
				var resolutions = new List<Resolution>();
				if (string.IsNullOrWhiteSpace(output))
				{
					return resolutions;
				}

				var matches = _resolutionRx.Matches(output);
				foreach (Match m in matches)
				{
					var split = m.Value.Split('x');
					var newRes = new Resolution(int.Parse(split[0]), int.Parse(split[1]));
					if (!resolutions.Contains(newRes, Resolution.WidthHeightComparer) && newRes.Width >= 800 && newRes.Height >= 600)
					{
						resolutions.Add(newRes);
					}
				}
				Log.Debug("Resolutions {@Resolutions}", resolutions);
				Log.Information("End enumerate resolutions");
				return resolutions.OrderBy(x => x.Height * x.Width);
			}
		}
	}
}