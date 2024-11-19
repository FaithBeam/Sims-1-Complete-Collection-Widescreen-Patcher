using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using sims_iff.Models;
using sims_iff.Models.ResourceContent.CARR;
using sims_iff.Models.ResourceContent.Str;
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
    private JobInfo? _selectedJob;
    private readonly ObservableAsPropertyHelper<Iff?>? _workIff;
    private readonly ObservableAsPropertyHelper<bool>? _extractWorkIffIsExecuting;

    private string? _jobName;
    private string? _maleSkin;
    private string? _femaleSkin;
    private string? _texture;
    private string? _accessory;

    private int? _friendsNeeded;
    private int? _cooking;
    private int? _mechanical;
    private int? _charisma;
    private int? _body;
    private int? _logic;
    private int? _creativity;

    private int? _hunger;
    private int? _comfort;
    private int? _hygiene;
    private int? _bladder;
    private int? _energy;
    private int? _fun;
    private int? _social;

    private int? _salary;
    private int? _beginTime;
    private int? _endTime;
    private CarType? _carType;

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
        this.WhenAnyValue(x => x.SelectedJob)
            .Subscribe(x =>
            {
                JobName = x?.JobName;
                MaleSkin = x?.MaleUniformMesh;
                FemaleSkin = x?.FemaleUniformMesh;
                Texture = x?.UniformSkin;
                Accessory = x?.Unknown4;

                FriendsNeeded = x?.FriendsRequired.Value;
                Cooking = x?.CookingSkillRequired.Value;
                Mechanical = x?.MechanicalSkillRequired.Value;
                Body = x?.BodySkillRequired.Value;
                Charisma = x?.CharismaRequired.Value;
                Logic = x?.LogicSkillRequired.Value;
                Creativity = x?.CreativitySkillRequired.Value;

                Hunger = x?.HungerDecay.Value;
                Comfort = x?.ComfortDecay.Value;
                Hygiene = x?.HygieneDecay.Value;
                Energy = x?.EnergyDecay.Value;
                Fun = x?.FunDecay.Value;
                Social = x?.SocialDecay.Value;
                Bladder = x?.BladderDecay.Value;

                Salary = x?.Salary.Value;
                BeginTime = x?.StartTime.Value;
                EndTime = x?.EndTime.Value;
                CarType = x?.CarType;
            });
    }

    public ReadOnlyObservableCollection<Resource> Careers => _careers;
    public ReadOnlyObservableCollection<JobInfo> Jobs => _jobs;

    private string? PathToExpansionShared => _pathToExpansionShared.Value;
    private string? PathToWorkIff => _pathToWorkIff.Value;
    private string? PathToExpansionSharedFar => _pathToExpansionSharedFar.Value;

    private bool ExtractWorkIffIsExecuting => _extractWorkIffIsExecuting?.Value ?? false;
    private Iff? WorkIff => _workIff?.Value;
    public Resource? SelectedCareer
    {
        get => _selectedCareer;
        set => this.RaiseAndSetIfChanged(ref _selectedCareer, value);
    }

    public JobInfo? SelectedJob
    {
        get => _selectedJob;
        set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
    }

    public string? JobName
    {
        get => _jobName;
        set => this.RaiseAndSetIfChanged(ref _jobName, value);
    }

    public string? MaleSkin
    {
        get => _maleSkin;
        set => this.RaiseAndSetIfChanged(ref _maleSkin, value);
    }

    public string? FemaleSkin
    {
        get => _femaleSkin;
        set => this.RaiseAndSetIfChanged(ref _femaleSkin, value);
    }

    public string? Texture
    {
        get => _texture;
        set => this.RaiseAndSetIfChanged(ref _texture, value);
    }

    public string? Accessory
    {
        get => _accessory;
        set => this.RaiseAndSetIfChanged(ref _accessory, value);
    }

    public int? FriendsNeeded
    {
        get => _friendsNeeded;
        set => this.RaiseAndSetIfChanged(ref _friendsNeeded, value);
    }

    public int? Cooking
    {
        get => _cooking;
        set => this.RaiseAndSetIfChanged(ref _cooking, value);
    }

    public int? Mechanical
    {
        get => _mechanical;
        set => this.RaiseAndSetIfChanged(ref _mechanical, value);
    }

    public int? Charisma
    {
        get => _charisma;
        set => this.RaiseAndSetIfChanged(ref _charisma, value);
    }

    public int? Body
    {
        get => _body;
        set => this.RaiseAndSetIfChanged(ref _body, value);
    }

    public int? Logic
    {
        get => _logic;
        set => this.RaiseAndSetIfChanged(ref _logic, value);
    }

    public int? Creativity
    {
        get => _creativity;
        set => this.RaiseAndSetIfChanged(ref _creativity, value);
    }

    public int? Hunger
    {
        get => _hunger;
        set => this.RaiseAndSetIfChanged(ref _hunger, value);
    }

    public int? Comfort
    {
        get => _comfort;
        set => this.RaiseAndSetIfChanged(ref _comfort, value);
    }

    public int? Hygiene
    {
        get => _hygiene;
        set => this.RaiseAndSetIfChanged(ref _hygiene, value);
    }

    public int? Bladder
    {
        get => _bladder;
        set => this.RaiseAndSetIfChanged(ref _bladder, value);
    }

    public int? Energy
    {
        get => _energy;
        set => this.RaiseAndSetIfChanged(ref _energy, value);
    }

    public int? Fun
    {
        get => _fun;
        set => this.RaiseAndSetIfChanged(ref _fun, value);
    }

    public int? Social
    {
        get => _social;
        set => this.RaiseAndSetIfChanged(ref _social, value);
    }

    public int? Salary
    {
        get => _salary;
        set => this.RaiseAndSetIfChanged(ref _salary, value);
    }

    public int? BeginTime
    {
        get => _beginTime;
        set => this.RaiseAndSetIfChanged(ref _beginTime, value);
    }

    public int? EndTime
    {
        get => _endTime;
        set => this.RaiseAndSetIfChanged(ref _endTime, value);
    }

    public CarType? CarType
    {
        get => _carType;
        set => this.RaiseAndSetIfChanged(ref _carType, value);
    }

    public List<CarType> CarTypes { get; } =
        new()
        {
            sims_iff.Models.ResourceContent.Str.CarType.Coupe,
            sims_iff.Models.ResourceContent.Str.CarType.Jeep,
            sims_iff.Models.ResourceContent.Str.CarType.Cruiser,
            sims_iff.Models.ResourceContent.Str.CarType.Sedan,
            sims_iff.Models.ResourceContent.Str.CarType.Suv,
            sims_iff.Models.ResourceContent.Str.CarType.TownCar,
            sims_iff.Models.ResourceContent.Str.CarType.Bentley,
            sims_iff.Models.ResourceContent.Str.CarType.Junker,
            sims_iff.Models.ResourceContent.Str.CarType.Limo,
            sims_iff.Models.ResourceContent.Str.CarType.Truck,
            sims_iff.Models.ResourceContent.Str.CarType.Circus,
            sims_iff.Models.ResourceContent.Str.CarType.ClownCar,
        };

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
