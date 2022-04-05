using System.IO;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Far.Models
{
    public class ScalePanelback : IJob
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
            using (var img = new MagickImage(Bytes))
            {
                var left = img.Clone(0, 0, 286, 100);
                var middle = img.Clone(left.Width, 0, 500, 100);
                var right = img.Clone(left.Width + middle.Width, 0, 18, 100);
                middle.Resize(new MagickGeometry(Width - left.Width - right.Width, Height) {IgnoreAspectRatio = true});
                left.Page = new MagickGeometry("+0+0");
                middle.Page = new MagickGeometry($"+{left.Width}+0");
                right.Page = new MagickGeometry($"+{left.Width + middle.Width}+0");
                using (var images = new MagickImageCollection())
                {
                    images.Add(left);
                    images.Add(middle);
                    images.Add(right);
                    var merged = images.Merge();
                    merged.Depth = 8;
                    merged.Settings.Compression = CompressionMethod.RLE;
                    merged.Settings.Format = MagickFormat.Bmp3;
                    merged.ColorType = ColorType.Palette;
                    merged.Alpha(AlphaOption.Off);
                    merged.Write(Output);
                }
            }
        }
    }
}