using ImageMagick;

namespace Sims1WidescreenPatcher.Images.Models;

public class ScalePanelbackJob : BaseImageProcessingJob
{
    public ScalePanelbackJob(byte[] bytes, string output, int width, int height) : base(bytes, output, width, height)
    {
    }

    public override void Run()
    {
        using var image = new MagickImage(ImageBytes);
        var left = image.Clone(0, 0, 286, 100);
        var middle = image.Clone(left.Width, 0, 500, 100);
        var right = image.Clone(left.Width + middle.Width, 0, 18, 100);
        middle.Resize(new MagickGeometry(Width - left.Width - right.Width, Height) { IgnoreAspectRatio = true });
        left.Page = new MagickGeometry("+0+0");
        middle.Page = new MagickGeometry($"+{left.Width}+0");
        right.Page = new MagickGeometry($"+{left.Width + middle.Width}+0");
        using var imageCollection = new MagickImageCollection();
        imageCollection.Add(left);
        imageCollection.Add(middle);
        imageCollection.Add(right);
        var merged = imageCollection.Merge();
        SetCommonBmpSettings((MagickImage)merged);
        merged.Write(Output);
    }
}