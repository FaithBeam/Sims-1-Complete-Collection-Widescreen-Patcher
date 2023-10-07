using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public class AspectRatio : IEqualityComparer<AspectRatio>, IComparable<AspectRatio>
{
    protected bool Equals(AspectRatio other)
    {
        return Numerator == other.Numerator && Denominator == other.Denominator;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AspectRatio) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Numerator * 397) ^ Denominator;
        }
    }

    public int Numerator;
    public int Denominator;
    
    public AspectRatio(int width, int height)
    {
        CalculateAspectRatio(width, height);
    }

    public override string ToString()
    {
        return $"{Numerator}:{Denominator}";
    }

    public static bool operator ==(AspectRatio obj1, AspectRatio obj2)
    {
        return (obj1.Numerator == obj2.Numerator
             && obj1.Denominator == obj2.Denominator);
    }

    public static bool operator !=(AspectRatio obj1, AspectRatio obj2)
    {
        return !(obj1 == obj2);
    }

    private void CalculateAspectRatio(int width, int height)
    {
        var gcd = BigInteger.GreatestCommonDivisor(width, height);
        Numerator = width / (int)gcd;
        Denominator = height / (int)gcd;
    }

    public bool Equals(AspectRatio x, AspectRatio y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Numerator == y.Numerator && x.Denominator == y.Denominator;
    }

    public int GetHashCode(AspectRatio obj)
    {
        unchecked
        {
            return (obj.Numerator * 397) ^ obj.Denominator;
        }
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