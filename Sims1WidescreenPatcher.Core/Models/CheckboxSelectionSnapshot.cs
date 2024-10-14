namespace Sims1WidescreenPatcher.Core.Models;

public class CheckboxSelectionSnapshot : IEquatable<CheckboxSelectionSnapshot>
{
    private readonly bool[] _vms;

    public CheckboxSelectionSnapshot(params bool[] vms)
    {
        _vms = vms;
    }

    public bool Equals(CheckboxSelectionSnapshot? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_vms.Length != other._vms.Length) return false;

        for (int i = 0; i < _vms.Length; i++)
        {
            if (_vms[i] != other._vms[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CheckboxSelectionSnapshot)obj);
    }

    public override int GetHashCode()
    {
        return _vms.GetHashCode();
    }
}