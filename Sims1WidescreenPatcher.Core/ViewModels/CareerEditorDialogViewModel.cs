using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using sims_iff.Models;
using sims_iff.Models.ResourceContent.CARR;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICareerEditorTabViewModel { }

public class CareerEditorDialogViewModel : ViewModelBase, ICareerEditorTabViewModel
{
    private IAppState AppState { get; }
    private readonly IFar _far;
    private readonly ObservableAsPropertyHelper<string?> _pathToExpansionShared;
    private readonly ObservableAsPropertyHelper<string?> _pathToExpansionSharedFar;
    private readonly ObservableAsPropertyHelper<string?> _pathToWorkIff;
    private readonly ReadOnlyObservableCollection<Resource> _careers;
    private readonly ReadOnlyObservableCollection<JobInfo> _jobs;
    private Resource? _selectedCareer;
    private readonly ObservableAsPropertyHelper<Iff?>? _workIff;
    private readonly ObservableAsPropertyHelper<bool>? _extractWorkIffIsExecuting;

    public CareerEditorDialogViewModel(IAppState appState, IFar far)
    {
        AppState = appState;
        _far = far;

        _pathToExpansionShared = this.WhenAnyValue(x => x.AppState.SimsExePath)
            .Select(GetPathToExpansionSharedDir)
            .ToProperty(this, x => x.PathToExpansionShared);
        _pathToExpansionSharedFar = this.WhenAnyValue(x => x.PathToExpansionShared)
            .Select(x =>
                string.IsNullOrWhiteSpace(x) ? null : Path.Combine(x, "ExpansionShared.far")
            )
            .ToProperty(this, x => x.PathToExpansionSharedFar);
        _pathToWorkIff = this.WhenAnyValue(x => x.PathToExpansionShared)
            .Select(GetPathToWorkIff)
            .ToProperty(this, x => x.PathToWorkIff);

        var canExecuteExtractWorkIff = this.WhenAnyValue(
            x => x.PathToWorkIff,
            x => x.ExtractWorkIffIsExecuting,
            selector: (p, _) => !string.IsNullOrWhiteSpace(p) && !GetWorkIffExtracted(p)
        );
        ExtractWorkIffCmd = ReactiveCommand.Create(ExtractWorkIff, canExecuteExtractWorkIff);
        _extractWorkIffIsExecuting = ExtractWorkIffCmd.IsExecuting.ToProperty(
            this,
            x => x.ExtractWorkIffIsExecuting
        );

        _workIff = this.WhenAnyValue(x => x.ExtractWorkIffIsExecuting, x => x.PathToWorkIff)
            .Select(x =>
                string.IsNullOrWhiteSpace(x.Item2) || !File.Exists(x.Item2)
                    ? null
                    : Iff.Read(x.Item2)
            )
            .ToProperty(this, x => x.WorkIff);

        SourceCache<Resource, int> iffSourceCache = new(x => x.Id);
        SourceList<JobInfo> jobInfoSourceList = new();
        var myOp = iffSourceCache
            .Connect()
            .Filter(x => x.TypeCode.Value == "CARR")
            .Bind(out _careers)
            .Subscribe();
        this.WhenAnyValue(x => x.SelectedCareer)
            .WhereNotNull()
            .Select(x => ((Carr)x.Content).JobInfos)
            .Subscribe(x =>
            {
                jobInfoSourceList.Edit(updater =>
                {
                    updater.Clear();
                    updater.AddRange(x);
                });
            });
        this.WhenAnyValue(x => x.WorkIff)
            .Select(x => x?.Resources)
            .Subscribe(x =>
                iffSourceCache.Edit(updater =>
                {
                    updater.Clear();
                    if (x is not null)
                    {
                        updater.AddOrUpdate(x);
                    }
                })
            );
        var op = jobInfoSourceList.Connect().Bind(out _jobs).Subscribe();
    }

    public ReadOnlyObservableCollection<Resource> Careers => _careers;
    public ReadOnlyObservableCollection<JobInfo> Jobs => _jobs;

    private string? PathToExpansionShared => _pathToExpansionShared.Value;
    private string? PathToWorkIff => _pathToWorkIff.Value;
    private string? PathToExpansionSharedFar => _pathToExpansionSharedFar.Value;

    private bool ExtractWorkIffIsExecuting => _extractWorkIffIsExecuting?.Value ?? false;
    public Iff? WorkIff => _workIff?.Value;
    public Resource? SelectedCareer
    {
        get => _selectedCareer;
        set => this.RaiseAndSetIfChanged(ref _selectedCareer, value);
    }
    public ReactiveCommand<Unit, Unit> ExtractWorkIffCmd { get; }

    private void ExtractWorkIff()
    {
        if (
            string.IsNullOrWhiteSpace(PathToExpansionSharedFar)
            || !File.Exists(PathToExpansionSharedFar)
        )
        {
            return;
        }
        _far.PathToFar = PathToExpansionSharedFar;
        _far.ParseFar();
        _far.Extract(
            _far.Manifest.ManifestEntries.First(x => x.Filename == "work.iff"),
            PathToExpansionShared
        );
    }

    private static bool GetWorkIffExtracted(string? pathToWorkIff) => File.Exists(pathToWorkIff);

    private static string? GetPathToWorkIff(string? expansionSharedDir) =>
        string.IsNullOrWhiteSpace(expansionSharedDir)
            ? null
            : Path.Combine(expansionSharedDir, "work.iff");

    private static string? GetPathToExpansionSharedDir(string? pathToSimsExe)
    {
        if (string.IsNullOrWhiteSpace(pathToSimsExe))
        {
            return null;
        }
        var directory = Path.GetDirectoryName(pathToSimsExe);
        return string.IsNullOrWhiteSpace(directory)
            ? null
            : Path.Combine(directory, "ExpansionShared");
    }
}
