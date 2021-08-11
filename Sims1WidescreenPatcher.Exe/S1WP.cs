using PatternFinder;
using Serilog;
using Sims1WidescreenPatcher.IO;
using System;
using System.IO;

namespace Sims1WidescreenPatcher.Exe
{
    public class S1WP
    {
        private const string WidthPattern = "20 03";
        private const string HeightPattern = "58 02";
        private const string BetweenPattern = "?? ?? ?? ?? ??";
        private readonly string _path;
        private readonly int _width;
        private readonly int _height;

        public S1WP(string path, int width, int height)
        {
            _path = path;
            _width = width;
            _height = height;
        }

        public bool ValidFile()
        {
            if (string.IsNullOrWhiteSpace(_path) || !File.Exists(_path))
            {
                Log.Debug($"{_path} is not valid.");
                return false;
            }

            var pattern = Pattern.Transform(WidthPattern + " " + BetweenPattern + " " + HeightPattern);
            var bytes = File.ReadAllBytes(_path);

            if (Pattern.Find(bytes, pattern, out long _))
            {
                Log.Debug($"{_path} is valid.");
                return true;
            }
            else
            {
                Log.Debug($"Could not find pattern in {_path}.");
                return false;
            }
        }

        public void Patch()
        {
            Log.Debug($"Begin patching {_path}.");
            if (!string.IsNullOrWhiteSpace(_path) && File.Exists(_path))
            {
                var pattern = Pattern.Transform(WidthPattern + " " + BetweenPattern + " " + HeightPattern);
                var bytes = File.ReadAllBytes(_path);

                if (!Pattern.Find(bytes, pattern, out var foundOffset)) return;
                Log.Debug($"Found {WidthPattern} {BetweenPattern} {HeightPattern} at {foundOffset}.");
                FileHelper.BackupFile(_path);

                var width = BitConverter.GetBytes(_width);
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                var height = BitConverter.GetBytes(_height);
                bytes[foundOffset + 2 + BetweenPattern.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + BetweenPattern.Trim().Split().Length + 1] = height[1];

                File.SetAttributes(_path, FileAttributes.Normal);
                File.WriteAllBytes(_path, bytes);
                Log.Debug($"Patched {_path}.");
            }
            else
            {
                Log.Debug($"{_path} doesn't exist.");
            }
        }
    }
}
