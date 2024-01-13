using Sims1WidescreenPatcher.Core.Events;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public class ProgressService : IProgressService
{
    public event EventHandler<NewProgressEventArgs>? NewProgressEventHandler;
    public event EventHandler<NewUninstallEventArgs>? NewUninstallEventHandler;

    public void UpdateProgress(double pct, string status, string status2)
    {
        var args = new NewProgressEventArgs(pct, status, status2);
        var handler = NewProgressEventHandler;
        handler?.Invoke(this, args);
    }

    public void UpdateProgress(double pct)
    {
        UpdateProgress(pct, string.Empty, string.Empty);
    }

    public void UpdateUninstall()
    {
        var arg = new NewUninstallEventArgs();
        var handler = NewUninstallEventHandler;
        handler?.Invoke(this, arg);
    }
}
