using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Utilities.Services
{
    public interface IProgressService
    {
        event EventHandler<NewProgressEventArgs>? NewProgressEventHandler;

        void UpdateProgress(double pct);
        void UpdateProgress(double pct, string status, string status2);
    }
}