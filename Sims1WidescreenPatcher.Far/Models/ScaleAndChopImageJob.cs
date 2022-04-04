using System.IO;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class ScaleAndChopImageJob : IJob
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
            using (var baseImg = new MagickImage(Bytes))
            using (var resizeImg = new MagickImage(Bytes))
            {
                var chopGeometry = new MagickGeometry()
                {
                    X = baseImg.Width - 10,
                    Width = 10
                };
                baseImg.Chop(chopGeometry);
                
                var size = new MagickGeometry(Width, Height)
                {
                    IgnoreAspectRatio = true
                };
                resizeImg.Resize(size);
                resizeImg.Composite(baseImg, Gravity.West);
                resizeImg.Depth = 8;
                resizeImg.Settings.Compression = CompressionMethod.RLE;
                resizeImg.Settings.Format = MagickFormat.Bmp3;
                resizeImg.ColorType = ColorType.Palette;
                resizeImg.Alpha(AlphaOption.Off);
                resizeImg.Write(Output);
            }
        }
    }
}