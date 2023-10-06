using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public class AspectRatio : IEqualityComparer<AspectRatio>, IComparable<AspectRatio>
{
    protected bool Equals(AspectRatio other)
    {
        return _numerator == other._numerator && _denominator == other._denominator;
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
            return (_numerator * 397) ^ _denominator;
        }
    }

    private int _numerator;
    private int _denominator;
    
    public AspectRatio(int width, int height)
    {
        CalculateAspectRatio(width, height);
    }

    public override string ToString()
    {
        return $"{_numerator}:{_denominator}";
    }

    public static bool operator ==(AspectRatio obj1, AspectRatio obj2)
    {
        return (obj1._numerator == obj2._numerator
             && obj1._denominator == obj2._denominator);
    }

    public static bool operator !=(AspectRatio obj1, AspectRatio obj2)
    {
        return !(obj1 == obj2);
    }

    private void CalculateAspectRatio(int width, int height)
    {
        var gcd = BigInteger.GreatestCommonDivisor(width, height);
        _numerator = width / (int)gcd;
        _denominator = height / (int)gcd;
    }

    public bool Equals(AspectRatio x, AspectRatio y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x._numerator == y._numerator && x._denominator == y._denominator;
    }

    public int GetHashCode(AspectRatio obj)
    {
        unchecked
        {
            return (obj._numerator * 397) ^ obj._denominator;
        }
    }

    public int CompareTo(AspectRatio? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var numeratorComparison = _numerator.CompareTo(other._numerator);
        if (numeratorComparison != 0) return numeratorComparison;
        return _denominator.CompareTo(other._denominator);
    }
}