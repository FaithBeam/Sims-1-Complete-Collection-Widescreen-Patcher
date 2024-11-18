using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using sims_iff.Models;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICareerEditorTabViewModel { }

public class CareerEditorDialogViewModel : ViewModelBase, ICareerEditorTabViewModel
{
    private IAppState AppState { get; }
    private readonly IFar _far;
    private string? _pathToExpansionShared;
    private string? _pathToExpansionSharedFar;
    private string? _pathToWorkIff;
    private bool _extractWorkIffCmdEnabled;
    private SourceCache<Resource, int> _iffSourceCache;
    private readonly ReadOnlyObservableCollection<Resource> _careers;

    private Iff? _workIff;

    public CareerEditorDialogViewModel(IAppState appState, IFar far)
    {
        AppState = appState;
        _far = far;
        (_pathToExpansionShared, _pathToExpansionSharedFar) = GetPathToExpansionSharedDir(
            appState.SimsExePath
        );
        _pathToWorkIff = GetPathToWorkIff(_pathToExpansionShared);
        ExtractWorkIffCmdEnabled = !GetWorkIffExtracted(_pathToWorkIff);

        ExtractWorkIffCmd = ReactiveCommand.Create(ExtractWorkIff);
        ExtractWorkIffCmd
            .IsExecuting.Where(x => !x)
            .Subscribe(_ => ExtractWorkIffCmdEnabled = !File.Exists(_pathToWorkIff));
        
        _workIff = Iff.Read(_pathToWorkIff!);
        _iffSourceCache = new SourceCache<Resource, int>(x => x.Id);
        var myOp = _iffSourceCache.Connect()
            .Filter(x => x.TypeCode.Value == "CARR")
            .Bind(out _careers)
            .Subscribe();
        _iffSourceCache.AddOrUpdate(_workIff.Resources);
    }

    public ReadOnlyObservableCollection<Resource> Careers => _careers;

    public ReactiveCommand<Unit, Unit> ExtractWorkIffCmd { get; }

    public bool ExtractWorkIffCmdEnabled
    {
        get => _extractWorkIffCmdEnabled;
        set => this.RaiseAndSetIfChanged(ref _extractWorkIffCmdEnabled, value);
    }

    private void ExtractWorkIff()
    {
        if (
            string.IsNullOrWhiteSpace(_pathToExpansionSharedFar)
            || !File.Exists(_pathToExpansionSharedFar)
        )
        {
            return;
        }
        _far.PathToFar = _pathToExpansionSharedFar;
        _far.ParseFar();
        _far.Extract(
            _far.Manifest.ManifestEntries.First(x => x.Filename == "work.iff"),
            _pathToExpansionShared
        );
    }

    private static bool GetWorkIffExtracted(string? pathToWorkIff) => File.Exists(pathToWorkIff);

    private static string? GetPathToWorkIff(string? expansionSharedDir) =>
        string.IsNullOrWhiteSpace(expansionSharedDir)
            ? null
            : Path.Combine(expansionSharedDir, "work.iff");

    private static (string? dir, string? file) GetPathToExpansionSharedDir(string? pathToSimsExe)
    {
        if (string.IsNullOrWhiteSpace(pathToSimsExe))
        {
            return (null, null);
        }
        var directory = Path.GetDirectoryName(pathToSimsExe);
        if (string.IsNullOrWhiteSpace(directory))
        {
            return (null, null);
        }
        var pathToExpansionSharedDir = Path.Combine(directory, "ExpansionShared");
        if (string.IsNullOrWhiteSpace(pathToExpansionSharedDir))
        {
            return (null, null);
        }
        var pathToExpansionSharedFar = Path.Combine(
            pathToExpansionSharedDir,
            "ExpansionShared.far"
        );
        return (pathToExpansionSharedDir, pathToExpansionSharedFar);
    }
}
