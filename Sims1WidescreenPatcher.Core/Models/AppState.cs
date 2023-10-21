using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.Models;

public class AppState : ReactiveObject, IAppState
{
    private string? _simsExePath;

    public string? SimsExePath
    {
        get => _simsExePath;
        set => this.RaiseAndSetIfChanged(ref _simsExePath, value);
    }

    public bool SimsExePathExists => File.Exists(SimsExePath);
}