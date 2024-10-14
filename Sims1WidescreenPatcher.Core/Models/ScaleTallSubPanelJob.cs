using ImageMagick;

namespace Sims1WidescreenPatcher.Core.Models;

public class ScaleTallSubPanelJob : BaseImageProcessingJob
{
    public override void Run()
    {
        base.Run();
        
        using var image = new MagickImage(ImageBytes!, MagickFormat.Tga);
        image.Resize(new MagickGeometry((uint)Width, (uint)Height) { IgnoreAspectRatio = true });
        image.Depth = 32;
        image.Settings.Compression = CompressionMethod.RLE;
        image.Settings.Format = MagickFormat.Tga;
        image.Write(Output!);
    }
}