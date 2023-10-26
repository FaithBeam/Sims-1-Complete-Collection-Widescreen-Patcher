namespace Sims1WidescreenPatcher.Core.Models;

public interface IAppState
{
    string? SimsExePath { get; set; }
    bool SimsExePathExists { get; }
    Resolution? Resolution { get; set; }
}