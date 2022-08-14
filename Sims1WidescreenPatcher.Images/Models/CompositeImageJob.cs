using ImageMagick;

namespace Sims1WidescreenPatcher.Images.Models;

public class CompositeImageJob : BaseImageProcessingJob
{
    /// <summary>
    /// Color should be a hex color code like #113355
    /// </summary>
    private readonly string _color;

    public CompositeImageJob(string color, byte[] bytes, string output, int width, int height) : base(bytes, output,
        width, height)
    {
        _color = color;
    }

    public override void Run()
    {
        using var image = new MagickImage(ImageBytes);
        using var background = new MagickImage(new MagickColor(_color), Width, Height);
        background.Composite(image, Gravity.Center);
        SetCommonBmpSettings(background);
        background.Write(Output);
    }
}