using System.IO;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class ScaleImageJob : IJob
    {
        public byte[] Bytes { get; set; }
        public string Output { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Extract()
        {
            var destDir = Path.GetDirectoryName(Output);
            DirectoryHelper.CreateDirectory(destDir);
            
            Log.Debug("Scale bytes to {Output}. Width: {Width}, Height: {Height}", Output, Width, Height);
            using (var image = new MagickImage(Bytes))
            {
                var size = new MagickGeometry(Width, Height)
                {
                    IgnoreAspectRatio = true
                };
                image.Resize(size);
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