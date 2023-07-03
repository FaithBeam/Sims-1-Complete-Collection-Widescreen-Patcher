using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Utilities.Services;

public class ProgressService : IProgressService
{
    public event EventHandler<NewProgressEventArgs>? NewProgressEventHandler;

    public void UpdateProgress(double pct)
    {
        var args = new NewProgressEventArgs(pct);
        var handler = NewProgressEventHandler;
        handler?.Invoke(this, args);
    }
}
