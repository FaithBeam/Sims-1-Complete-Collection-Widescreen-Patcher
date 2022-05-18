using System.IO;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class ScaleJob : IJob
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
            using (var image = new MagickImage(Bytes, MagickFormat.Tga))
            {
                image.Resize(new MagickGeometry(Width, Height) {IgnoreAspectRatio = true});
                image.Depth = 32;
                image.Settings.Compression = CompressionMethod.RLE;
                image.Settings.Format = MagickFormat.Tga;
                image.Write(Output);
            }
        }
    }
}