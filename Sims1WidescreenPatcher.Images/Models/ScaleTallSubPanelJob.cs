using ImageMagick;

namespace Sims1WidescreenPatcher.Images.Models;

public class ScaleTallSubPanelJob : BaseImageProcessingJob
{
    public ScaleTallSubPanelJob(byte[] bytes, string output, int width, int height) : base(bytes, output, width, height)
    {
    }

    public override void Run()
    {
        using var image = new MagickImage(ImageBytes, MagickFormat.Tga);
        image.Resize(new MagickGeometry(Width, Height) { IgnoreAspectRatio = true });
        image.Depth = 32;
        image.Settings.Compression = CompressionMethod.RLE;
        image.Settings.Format = MagickFormat.Tga;
        image.Write(Output);
    }
}