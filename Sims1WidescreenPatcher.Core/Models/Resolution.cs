namespace Sims1WidescreenPatcher.Core.Models;

public class Resolution : IEquatable<Resolution>
{
    public uint Width { get; }
    public uint Height { get; }
    public AspectRatio AspectRatio { get; }

    public Resolution(uint width, uint height)
    {
        Width = width;
        Height = height;
        AspectRatio = new AspectRatio(width, height);
    }

    public override string ToString()
    {
        return $"{Width}x{Height} ({AspectRatio})";
    }

    public bool Equals(Resolution? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Resolution)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }
}