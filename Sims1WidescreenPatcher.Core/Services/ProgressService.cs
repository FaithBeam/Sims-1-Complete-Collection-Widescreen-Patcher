using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Sims1WidescreenPatcher.Core.Events;
using Sims1WidescreenPatcher.Core.Services.Interfaces;

namespace Sims1WidescreenPatcher.Core.Services;

public interface IProgressService
{
    void UpdateProgress(double pct);
    void UpdateProgress(double pct, string status, string status2);
    void UpdateUninstall();
    IObservable<Unit> Uninstall { get; }
    IObservable<NewProgressEventArgs> NewProgressObservable { get; }
}

public class ProgressService : IProgressService
{
    private readonly Subject<Unit> _uninstallSubject = new();
    public IObservable<Unit> Uninstall => _uninstallSubject.AsObservable();

    private readonly Subject<NewProgressEventArgs> _progressSubject = new();
    public IObservable<NewProgressEventArgs> NewProgressObservable =>
        _progressSubject.AsObservable();

    public void UpdateProgress(double pct, string status, string status2) =>
        _progressSubject.OnNext(new NewProgressEventArgs(pct, status, status2));

    public void UpdateProgress(double pct) =>
        _progressSubject.OnNext(new NewProgressEventArgs(pct, "", ""));

    public void UpdateUninstall() => _uninstallSubject.OnNext(Unit.Default);
}
