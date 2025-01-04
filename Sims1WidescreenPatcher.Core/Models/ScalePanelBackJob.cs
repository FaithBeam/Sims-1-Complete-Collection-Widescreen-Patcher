using ImageMagick;

namespace Sims1WidescreenPatcher.Core.Models;

public class ScalePanelBackJob : BaseImageProcessingJob
{
    public override void Run()
    {
        base.Run();

        using var image = new MagickImage(ImageBytes!);
        var left = image.CloneArea(0, 0, 286, 100);
        var middle = image.CloneArea((int)left.Width, 0, 500, 100);
        var right = image.CloneArea((int)left.Width + (int)middle.Width, 0, 18, 100);
        middle.Resize(
            new MagickGeometry((uint)Width - left.Width - right.Width, (uint)Height)
            {
                IgnoreAspectRatio = true,
            }
        );
        left.Page = new MagickGeometry("+0+0");
        middle.Page = new MagickGeometry($"+{left.Width}+0");
        right.Page = new MagickGeometry($"+{left.Width + middle.Width}+0");
        using var imageCollection = new MagickImageCollection();
        imageCollection.Add(left);
        imageCollection.Add(middle);
        imageCollection.Add(right);
        var merged = imageCollection.Merge();
        SetCommonBmpSettings((MagickImage)merged);
        merged.Write(Output!);
    }
}
