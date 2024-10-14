using System.Reactive;
using Sims1WidescreenPatcher.Core.Events;

namespace Sims1WidescreenPatcher.Core.Services.Interfaces
{
    public interface IProgressService
    {
        void UpdateProgress(double pct);
        void UpdateProgress(double pct, string status, string status2);
        void UpdateUninstall();
        IObservable<Unit> Uninstall { get; }
        IObservable<NewProgressEventArgs> NewProgressObservable { get; }
    }
}