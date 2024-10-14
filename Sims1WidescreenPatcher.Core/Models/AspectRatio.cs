using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public record AspectRatio(int Width, int Height) : IComparable<AspectRatio>
{
    private readonly BigInteger _gcd = BigInteger.GreatestCommonDivisor(Width, Height);
    public int Numerator => Width / (int)_gcd;
    public int Denominator => Height / (int)_gcd;

    public override string ToString() => $"{Numerator}:{Denominator}";

    public virtual bool Equals(AspectRatio? other) =>
        other is not null && Numerator == other.Numerator && Denominator == other.Denominator;

    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

    public int CompareTo(AspectRatio? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var numeratorComparison = Numerator.CompareTo(other.Numerator);
        if (numeratorComparison != 0) return numeratorComparison;
        return Denominator.CompareTo(other.Denominator);
    }
}