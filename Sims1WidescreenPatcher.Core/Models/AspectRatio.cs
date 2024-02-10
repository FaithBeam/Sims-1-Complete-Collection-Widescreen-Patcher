using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public class AspectRatio : IEquatable<AspectRatio>, IComparable<AspectRatio>
{
    public uint Numerator;
    public uint Denominator;
    
    public AspectRatio(uint width, uint height)
    {
        CalculateAspectRatio(width, height);
    }

    public override string ToString()
    {
        return $"{Numerator}:{Denominator}";
    }

    private void CalculateAspectRatio(uint width, uint height)
    {
        var gcd = BigInteger.GreatestCommonDivisor(width, height);
        Numerator = (uint)(width / (int)gcd);
        Denominator = (uint)(height / (int)gcd);
    }

    public bool Equals(AspectRatio? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Numerator == other.Numerator && Denominator == other.Denominator;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AspectRatio)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Numerator, Denominator);
    }

    public int CompareTo(AspectRatio? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var numeratorComparison = Numerator.CompareTo(other.Numerator);
        if (numeratorComparison != 0) return numeratorComparison;
        return Denominator.CompareTo(other.Denominator);
    }
}