namespace Sims1WidescreenPatcher.Core.Models;

public class Resolution
{
    private sealed class WidthHeightEqualityComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Width == y.Width && x.Height == y.Height;
        }

        public int GetHashCode(Resolution obj)
        {
            unchecked
            {
                return (obj.Width * 397) ^ obj.Height;
            }
        }
    }

    public static IEqualityComparer<Resolution>? WidthHeightComparer { get; } = new WidthHeightEqualityComparer();

    public int Width { get; }

    public int Height { get; }

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        if (Width == -1 && Height == -1)
        {
            return @"<Custom Resolution>";
        }
        return $"{Width}x{Height}";
    }
}