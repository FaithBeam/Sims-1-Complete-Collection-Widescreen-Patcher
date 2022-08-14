using ImageMagick;
using Serilog;

namespace Sims1WidescreenPatcher.Images.Models;

public abstract class BaseImageProcessingJob
{
    protected readonly byte[] ImageBytes;
    protected readonly string Output;
    protected readonly int Width;
    protected readonly int Height;

    protected BaseImageProcessingJob(byte[] imageBytes, string output, int width, int height)
    {
        ImageBytes = imageBytes ?? throw new ArgumentNullException(nameof(imageBytes));
        Output = output ?? throw new ArgumentNullException(nameof(output));
        Width = width;
        Height = height;
        var destDir = Path.GetDirectoryName(Output);
        if (destDir == null || Directory.Exists(destDir)) return;
        Log.Information("Created directory {@Directory}", destDir);
        Directory.CreateDirectory(destDir);
    }

    public abstract void Run();

    protected static void SetCommonBmpSettings(MagickImage bmp)
    {
        bmp.Depth = 8;
        bmp.Settings.Compression = CompressionMethod.RLE;
        bmp.Settings.Format = MagickFormat.Bmp3;
        bmp.ColorType = ColorType.Palette;
        bmp.Alpha(AlphaOption.Off);
    }
}