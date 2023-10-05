using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public class Resolution : IComparable<Resolution>
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
    public AspectRatio AspectRatio { get; }

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
        AspectRatio = new AspectRatio(width, height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height} ({AspectRatio})";
    }

    public int CompareTo(Resolution? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var widthComparison = Width.CompareTo(other.Width);
        if (widthComparison != 0) return widthComparison;
        return Height.CompareTo(other.Height);
    }
}