using PatternFinder;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Media;
using Sims1WidescreenPatcher.Models;
using Sims1WidescreenPatcher.Voodoo;
using System;
using System.IO;

namespace Sims1WidescreenPatcher.Exe
{
    public class S1WP
    {
        readonly string WIDTH_PATTERN = "20 03";
        readonly string HEIGHT_PATTERN = "58 02";
        const string BETWEEN_PATTERN = "?? ?? ?? ?? ??";
        readonly PatchOptions _options;

        public S1WP(PatchOptions options)
        {
            _options = options;
        }

        public bool ValidFile()
        {
            var pattern = Pattern.Transform(WIDTH_PATTERN + " " + BETWEEN_PATTERN + " " + HEIGHT_PATTERN);
            var bytes = File.ReadAllBytes(_options.Path);

            if (Pattern.Find(bytes, pattern, out long _))
                return true;
            return false;
        }

        public void Patch()
        {
            if (!string.IsNullOrWhiteSpace(_options.Path))
            {
                if (File.Exists(_options.Path))
                {
                    if (_options.DgVoodooEnabled)
                        Voodoo2.ExtractVoodooZips(_options.Path);
                    if (EditFile())
                    {
                        var images = new Images(_options);
                        images.ExtractUigraphics();
                        images.CopyGraphics();
                    }
                }
            }
        }

        private bool EditFile()
        {
            var pattern = Pattern.Transform(WIDTH_PATTERN + " " + BETWEEN_PATTERN + " " + HEIGHT_PATTERN);
            var bytes = File.ReadAllBytes(_options.Path);

            if (Pattern.Find(bytes, pattern, out long foundOffset))
            {
                FileHelper.BackupFile(_options.Path);
                
                var width = BitConverter.GetBytes(_options.Width);
                bytes[foundOffset] = width[0];
                bytes[foundOffset + 1] = width[1];

                var height = BitConverter.GetBytes(_options.Height);
                bytes[foundOffset + 2 + BETWEEN_PATTERN.Trim().Split().Length] = height[0];
                bytes[foundOffset + 2 + BETWEEN_PATTERN.Trim().Split().Length + 1] = height[1];

                File.WriteAllBytes(_options.Path, bytes);
                return true;
            }

            return false;
        }
    }
}
