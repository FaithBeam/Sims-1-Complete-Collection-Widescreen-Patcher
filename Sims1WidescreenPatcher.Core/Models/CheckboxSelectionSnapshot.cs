namespace Sims1WidescreenPatcher.Core.Models;

public class CheckboxSelectionSnapshot : IEquatable<CheckboxSelectionSnapshot>
{
    public CheckboxSelectionSnapshot(params ValueTuple<string, bool>[] values)
    {
        States = new Dictionary<string, bool>();
        foreach (var vt in values)
        {
            States.Add(vt.Item1, vt.Item2);
        }
    }

    public readonly Dictionary<string, bool> States;

    public bool Equals(CheckboxSelectionSnapshot? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        foreach (var state in States)
        {
            if (!other.States.ContainsKey(state.Key))
            {
                return false;
            }

            if (other.States[state.Key] != state.Value)
            {
                return false;
            }
        }
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((CheckboxSelectionSnapshot)obj);
    }

    public override int GetHashCode()
    {
        return States.GetHashCode();
    }
}
