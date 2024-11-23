using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using sims_iff.Models.ResourceContent.Str;
using Sims.Far;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff;
using Sims1WidescreenPatcher.Core.ViewModels.Sims_Iff.ResourceContent.CARR;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICareerEditorTabViewModel { }

public class CareerEditorDialogViewModel : ViewModelBase, ICareerEditorTabViewModel
{
    private IAppState AppState { get; }
    private readonly IFar _far;
    private readonly ObservableAsPropertyHelper<string?> _pathToExpansionShared;
    private readonly ObservableAsPropertyHelper<string?> _pathToExpansionSharedFar;
    private readonly ObservableAsPropertyHelper<string?> _pathToWorkIff;
    private readonly ReadOnlyObservableCollection<ResourceViewModel> _careers;
    private readonly ReadOnlyObservableCollection<JobInfoViewModel> _jobs;
    private ResourceViewModel? _selectedCareer;
    private JobInfoViewModel? _selectedJob;
    private readonly ObservableAsPropertyHelper<IffViewModel?>? _workIff;
    private readonly ObservableAsPropertyHelper<bool>? _extractWorkIffIsExecuting;
    private IffPreset? _selectedPreset;
    private readonly ObservableAsPropertyHelper<string> _windowTitle;

    public CareerEditorDialogViewModel(IAppState appState, IFar far, IIffService iffService)
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

        ExtractWorkIffCmd = ReactiveCommand.Create(ExtractWorkIff);
        this.WhenAnyValue(x => x.PathToWorkIff, x => x.ExtractWorkIffIsExecuting)
            .Subscribe(x => ExtractWorkIffCmd.Execute().Subscribe());
        _extractWorkIffIsExecuting = ExtractWorkIffCmd.IsExecuting.ToProperty(
            this,
            x => x.ExtractWorkIffIsExecuting
        );

        _workIff = this.WhenAnyValue(x => x.ExtractWorkIffIsExecuting, x => x.PathToWorkIff)
            .Select(x =>
                string.IsNullOrWhiteSpace(x.Item2) || !File.Exists(x.Item2)
                    ? null
                    : iffService.Load(x.Item2)
            )
            .ToProperty(this, x => x.WorkIff);

        SourceCache<ResourceViewModel, int> iffSourceCache = new(x => x.Id);
        SourceList<JobInfoViewModel> jobInfoSourceList = new();
        var myOp = iffSourceCache
            .Connect()
            .Filter(x => x.TypeCode.Value == "CARR")
            .SortAndBind(
                out _careers,
                SortExpressionComparer<ResourceViewModel>.Ascending(x =>
                    ((CarrViewModel)x.Content).CareerInfo.CareerName
                )
            )
            .Subscribe();
        this.WhenAnyValue(x => x.SelectedCareer)
            .WhereNotNull()
            .Select(x => ((CarrViewModel)x.Content).JobInfos)
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

        var canExecuteSave = this.WhenAnyValue(
            x => x.PathToWorkIff,
            x => x.WorkIff,
            selector: (path, workIff) => !string.IsNullOrWhiteSpace(path) && workIff is not null
        );
        SaveCmd = ReactiveCommand.Create(
            () => iffService.Write(PathToWorkIff!, WorkIff!),
            canExecuteSave
        );

        // apply preset to job infos
        this.WhenAnyValue(x => x.SelectedPreset)
            .WhereNotNull()
            .Subscribe(x =>
                iffService.ApplyPreset(
                    Careers.SelectMany(c => ((CarrViewModel)c.Content).JobInfos),
                    (IffPreset)x!
                )
            );

        _windowTitle = this.WhenAnyValue(x => x.PathToWorkIff)
            .Select(x => $"Career Editor {(string.IsNullOrWhiteSpace(x) ? "" : $": {x}")}")
            .ToProperty(this, x => x.WindowTitle);
    }

    public ReadOnlyObservableCollection<ResourceViewModel> Careers => _careers;
    public ReadOnlyObservableCollection<JobInfoViewModel> Jobs => _jobs;

    private string? PathToExpansionShared => _pathToExpansionShared.Value;
    private string? PathToWorkIff => _pathToWorkIff.Value;
    private string? PathToExpansionSharedFar => _pathToExpansionSharedFar.Value;

    private bool ExtractWorkIffIsExecuting => _extractWorkIffIsExecuting?.Value ?? false;
    private IffViewModel? WorkIff => _workIff?.Value;
    public ResourceViewModel? SelectedCareer
    {
        get => _selectedCareer;
        set => this.RaiseAndSetIfChanged(ref _selectedCareer, value);
    }

    public JobInfoViewModel? SelectedJob
    {
        get => _selectedJob;
        set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
    }

    public List<IffPreset> Presets { get; } = new() { IffPreset.Default, IffPreset.Cheater };

    public IffPreset? SelectedPreset
    {
        get => _selectedPreset;
        set => this.RaiseAndSetIfChanged(ref _selectedPreset, value);
    }

    public string WindowTitle => _windowTitle.Value;

    public List<CarType> CarTypes { get; } =
        new()
        {
            CarType.Bentley,
            CarType.Circus,
            CarType.ClownCar,
            CarType.Coupe,
            CarType.Cruiser,
            CarType.Jeep,
            CarType.Junker,
            CarType.Limo,
            CarType.Sedan,
            CarType.Suv,
            CarType.TownCar,
            CarType.Truck,
        };

    public ReactiveCommand<Unit, Unit> ExtractWorkIffCmd { get; }
    public ReactiveCommand<Unit, Unit> SaveCmd { get; }

    private void ExtractWorkIff()
    {
        if (
            string.IsNullOrWhiteSpace(PathToExpansionSharedFar)
            || !File.Exists(PathToExpansionSharedFar)
            || string.IsNullOrWhiteSpace(PathToWorkIff)
            || File.Exists(PathToWorkIff)
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
