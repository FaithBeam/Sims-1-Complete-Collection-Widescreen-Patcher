using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.Models;

public class AppState : ReactiveObject, IAppState
{
    private string? _simsExePath;
    private Resolution? _resolution;

    public string? SimsExePath
    {
        get => _simsExePath;
        set => this.RaiseAndSetIfChanged(ref _simsExePath, value);
    }

    public bool SimsExePathExists => File.Exists(SimsExePath);

    public string SimsBackupPath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SimsExePath))
            {
                return "";
            }
            const string backupName = "Sims Backup.exe";
            var parent = Directory.GetParent(SimsExePath)!.ToString();
            return Path.Combine(parent, backupName);
        }
    }

    public bool SimsBackupExists
    {
        get
        {
            var backup = SimsBackupPath;
            return !string.IsNullOrWhiteSpace(backup) && File.Exists(backup);
        }
    }

    public Resolution? Resolution
    {
        get => _resolution;
        set => this.RaiseAndSetIfChanged(ref _resolution, value);
    }
}
