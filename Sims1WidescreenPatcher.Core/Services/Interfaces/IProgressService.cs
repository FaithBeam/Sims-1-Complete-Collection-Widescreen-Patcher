using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Core.Services.Interfaces
{
    public interface IProgressService
    {
        event EventHandler<NewProgressEventArgs>? NewProgressEventHandler;

        void UpdateProgress(double pct);
        void UpdateProgress(double pct, string status, string status2);
        event EventHandler<NewUninstallEventArgs>? NewUninstallEventHandler;
        void UpdateUninstall();
    }
}