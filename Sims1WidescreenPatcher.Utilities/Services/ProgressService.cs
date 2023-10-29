using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Utilities.Services;

public class ProgressService : IProgressService
{
    public event EventHandler<NewProgressEventArgs>? NewProgressEventHandler;

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
}
