using ImageMagick;
using Serilog;

namespace Sims1WidescreenPatcher.Core.Models;

public abstract class BaseImageProcessingJob
{
    public byte[]? ImageBytes;
    public string? Output;
    public string? BaseImageName;
    public int Width;
    public int Height;

    /// <summary>
    /// Overrides should call this base method before continuing
    /// </summary>
    public virtual void Run()
    {
        if (!string.IsNullOrWhiteSpace(Output))
        {
            BaseImageName = Path.GetFileName(Output);
        }
        var destDir = Path.GetDirectoryName(Output);
        if (destDir == null || Directory.Exists(destDir)) return;
        Log.Information("Created directory {@Directory}", destDir);
        Directory.CreateDirectory(destDir);
        if (ImageBytes is null || string.IsNullOrWhiteSpace(Output))
        {
            throw new Exception("ImagesBytes or Output is null");
        }
    }

    protected static void SetCommonBmpSettings(MagickImage bmp)
    {
        bmp.Depth = 8;
        bmp.Settings.Compression = CompressionMethod.RLE;
        bmp.Settings.Format = MagickFormat.Bmp3;
        bmp.ColorType = ColorType.Palette;
        bmp.Alpha(AlphaOption.Off);
    }
}