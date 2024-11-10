using ImageMagick;

namespace Sims1WidescreenPatcher.Core.Models;

public class CompositeImageJob : BaseImageProcessingJob
{
    /// <summary>
    /// Color should be a hex color code like #113355
    /// </summary>
    public string? Color;

    public override void Run()
    {
        base.Run();

        if (string.IsNullOrWhiteSpace(Color))
        {
            throw new Exception("Color is null");
        }

        using var image = new MagickImage(ImageBytes!);
        using var background = new MagickImage(new MagickColor(Color), (uint)Width, (uint)Height);
        background.Composite(image, Gravity.Center);
        SetCommonBmpSettings(background);
        background.Write(Output!);
    }
}
