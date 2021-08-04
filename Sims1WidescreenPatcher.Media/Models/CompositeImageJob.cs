using System.IO;
using System.Reflection;
using ImageMagick;
using Serilog;
using Sims1WidescreenPatcher.IO;

namespace Sims1WidescreenPatcher.Media.Models
{
    public class CompositeImageJob : IJob
    {
        public string Background { get; set; }
        public byte[] Bytes { get; set; }
        public string Output { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void DoWork()
        {
            var destDir = Path.GetDirectoryName(Output);
            DirectoryHelper.CreateDirectory(destDir);
            
            Log.Debug($"Composite {Background} with bytes to {Output}. Width: {Width}, Height: {Height}.");
            using (var compositeImage = new MagickImage(Bytes))
            {
                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"Sims1WidescreenPatcher.Media.Resources.{Background}"))
                using (var baseImage = new MagickImage(stream))
                {
                    var size = new MagickGeometry(Width, Height)
                    {
                        IgnoreAspectRatio = true
                    };
                    baseImage.Resize(size);
                    baseImage.Composite(compositeImage, Gravity.Center);
                    baseImage.Depth = 8;
                    baseImage.Settings.Compression = CompressionMethod.RLE;
                    baseImage.Settings.Format = MagickFormat.Bmp3;
                    baseImage.ColorType = ColorType.Palette;
                    baseImage.Alpha(AlphaOption.Off);
                    baseImage.Write(Output);
                }
            }
        }
    }
}