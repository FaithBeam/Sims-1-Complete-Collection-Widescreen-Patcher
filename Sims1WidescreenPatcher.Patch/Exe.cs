using System;
using System.IO;
using PatternFinder;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Patch
{
    public class Exe
    {
        private const string WidthPattern = "20 03";
        private const string HeightPattern = "58 02";
        private const string BetweenPattern = "?? ?? ?? ?? ??";

        public bool ValidFile(string path)
        {
            var pattern = Pattern.Transform(WidthPattern + " " + BetweenPattern + " " + HeightPattern);
            var bytes = File.ReadAllBytes(path);

            if (Pattern.Find(bytes, pattern, out long _))
            {
                Log.Debug("{Path} is valid", path);
                return true;
            }

            Log.Debug("Could not find pattern in {Path}", path);
            return false;
        }

        public void Patch(string path, int width, int height)
        {
            Log.Debug("Begin patching {Path}", path);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                var pattern = Pattern.Transform(WidthPattern + " " + BetweenPattern + " " + HeightPattern);
                var bytes = File.ReadAllBytes(path);

                if (!Pattern.Find(bytes, pattern, out var foundOffset)) return;
                Log.Debug("Found {WidthPattern} {BetweenPattern} {HeightPattern} at {FoundOffset}", WidthPattern, BetweenPattern, HeightPattern, foundOffset);
                FileHelper.BackupFile(path);

                var widthBytes = BitConverter.GetBytes(width);
                bytes[foundOffset] = widthBytes[0];
                bytes[foundOffset + 1] = widthBytes[1];

                var heightBytes = BitConverter.GetBytes(height);
                bytes[foundOffset + 2 + BetweenPattern.Trim().Split().Length] = heightBytes[0];
                bytes[foundOffset + 2 + BetweenPattern.Trim().Split().Length + 1] = heightBytes[1];

                File.SetAttributes(path, FileAttributes.Normal);
                File.WriteAllBytes(path, bytes);
                Log.Debug("Patched {Path}", path);
            }
            else
            {
                Log.Debug("{Path} doesn't exist", path);
            }
        }
    }
}
