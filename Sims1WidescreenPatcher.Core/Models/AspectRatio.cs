using System.Numerics;

namespace Sims1WidescreenPatcher.Core.Models;

public class AspectRatio : IEqualityComparer<AspectRatio>
{
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
}