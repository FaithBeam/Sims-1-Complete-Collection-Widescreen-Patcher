﻿namespace Sims1WidescreenPatcher.Core.Models;

public record Resolution(int Width, int Height) : IComparable<Resolution>
{
    public readonly AspectRatio AspectRatio = new(Width, Height);

    public override string ToString() => $"{Width}x{Height} ({AspectRatio})";

    public virtual bool Equals(Resolution? other) => other is not null && Width == other.Width &&
                                                     Height == other.Height && AspectRatio.Equals(other.AspectRatio);

    public override int GetHashCode() => HashCode.Combine(AspectRatio, Width, Height);

    public int CompareTo(Resolution? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var widthComparison = Width.CompareTo(other.Width);
        if (widthComparison != 0) return widthComparison;
        return Height.CompareTo(other.Height);
    }
}