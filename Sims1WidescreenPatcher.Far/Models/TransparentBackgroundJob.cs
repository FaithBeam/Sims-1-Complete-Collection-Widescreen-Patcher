using System.IO;
using System.Linq;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class TransparentBackgroundJob : IJob
    {
        public byte[] Bytes { get; set; }
        public string Output { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Extract()
        {
            var destDir = Path.GetDirectoryName(Output);
            DirectoryHelper.CreateDirectory(destDir);
            Log.Debug("Fix {Output} transparency.", Output);
            using (var image = new MagickImage(Bytes))
            {
                var color = image.Histogram()
                    .OrderByDescending(x => x.Value)
                    .First(x => x.Key.FuzzyEquals(MagickColor.FromRgb((byte) 59, (byte) 59, (byte) 120),
                        new Percentage(13)))
                    .Key;
                image.ColorFuzz = new Percentage(13);
                image.Opaque(color, MagickColor.FromRgb((byte) 255, (byte) 0, (byte) 255));
                image.Depth = 8;
                image.Settings.Compression = CompressionMethod.RLE;
                image.Settings.Format = MagickFormat.Bmp3;
                image.ColorType = ColorType.Palette;
                image.Alpha(AlphaOption.Off);
                image.Write(Output);
            }
        }
    }
}