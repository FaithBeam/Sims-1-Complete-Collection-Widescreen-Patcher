namespace Sims1WidescreenPatcher.Core.Models;

public interface IWrapper
{
    string Name { get; }
}

public class NoneWrapper : IWrapper
{
    public string Name => "None";

    public override string ToString()
    {
        return Name;
    }
}

public class DDrawCompatWrapper : IWrapper
{
    public DDrawCompatWrapper(string version)
    {
        Version = version;
    }

    public string Name => "DDrawCompat";
    public string Version { get; }

    public override string ToString()
    {
        return $"{Name} ({Version})";
    }
}

public class DgVoodoo2Wrapper : IWrapper
{
    public string Name => "DgVoodoo2";

    public override string ToString()
    {
        return Name;
    }
}
