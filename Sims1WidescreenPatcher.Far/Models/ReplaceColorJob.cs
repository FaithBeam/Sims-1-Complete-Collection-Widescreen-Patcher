using System.IO;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class ReplaceColorJob : IJob
    {
        public string ImagePath { get; set; }
        public MagickColor ReplaceColor { get; set; }
        public int Percentage { get; set; }
        public byte[] Bytes { get; set; }
        public string Output { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public void Extract()
        {
            var destDir = Path.GetDirectoryName(Output);
            DirectoryHelper.CreateDirectory(destDir);
            Log.Debug("Fix {Output} transparency", Output);
            using (var image = new MagickImage(Bytes))
            {
                image.ColorFuzz = new Percentage(Percentage);
                image.Opaque(ReplaceColor, MagickColor.FromRgb((byte) 255, (byte) 0, (byte) 255));
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