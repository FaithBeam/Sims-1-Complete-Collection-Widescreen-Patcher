using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Avalonia.Platform.Storage;
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
    private const string AboutLink =
        "https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki/Career-Editor";
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
    private IffPreset? _selectedPreset;
    private readonly ObservableAsPropertyHelper<string> _windowTitle;

    private readonly ObservableAsPropertyHelper<int?> _shiftLength;
    private readonly ObservableAsPropertyHelper<int?> _shiftHungerDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftComfortDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftHygieneDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftBladderDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftEnergyDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftFunDecay;
    private readonly ObservableAsPropertyHelper<int?> _shiftSocialDecay;

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
        var pathToWorkIffObs = this.WhenAnyValue(x => x.PathToExpansionSharedFar)
            .Select(ExtractWorkIff);

        ShowOpenFileDialogInteraction = new Interaction<Unit, IStorageFile?>();
        ShowOpenFileDialogCmd = ReactiveCommand.CreateFromTask(
            async () => (await ShowOpenFileDialogInteraction.Handle(Unit.Default))?.Path.LocalPath
        );

        var showFileDialogObs = this.WhenAnyObservable(x => x.ShowOpenFileDialogCmd).WhereNotNull();
        var mergedPathToWorkIffChangedObs = pathToWorkIffObs.Merge(showFileDialogObs);
        _pathToWorkIff = mergedPathToWorkIffChangedObs.ToProperty(this, x => x.PathToWorkIff);
        var workIffObs = this.WhenAnyValue(x => x.PathToWorkIff)
            .WhereNotNull()
            .Select(x =>
                string.IsNullOrWhiteSpace(x) || !File.Exists(x) ? null : iffService.Load(x)
            )
            .WhereNotNull();
        var canExecuteResetCmd = this.WhenAnyValue(
            x => x.PathToWorkIff,
            selector: p => !string.IsNullOrWhiteSpace(p) && File.Exists(p)
        );
        ResetCmd = ReactiveCommand.Create(
            () => iffService.Load(PathToWorkIff!),
            canExecuteResetCmd
        );
        var resetCmdObs = this.WhenAnyObservable(x => x.ResetCmd);
        workIffObs = workIffObs.Merge(resetCmdObs);
        _workIff = workIffObs.ToProperty(this, x => x.WorkIff);

        var canExecuteOpenWorkIffFolder = this.WhenAnyValue(
            x => x.PathToWorkIff,
            selector: p => !string.IsNullOrWhiteSpace(p)
        );
        OpenWorkIffFolderCmd = ReactiveCommand.Create(
            () =>
            {
                var processStartInfo = new ProcessStartInfo
                {
                    Arguments = Path.GetDirectoryName(PathToWorkIff),
                };

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    processStartInfo.FileName = "explorer.exe";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    processStartInfo.FileName = "xdg-open";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    processStartInfo.FileName = "open";
                }
                else
                {
                    throw new Exception("Unsupported platform");
                }

                processStartInfo.UseShellExecute = true;
                Process.Start(processStartInfo);
            },
            canExecuteOpenWorkIffFolder
        );

        ShowSaveFileDialogInteraction = new Interaction<Unit, IStorageFile?>();
        var canExecuteSaveAs = this.WhenAnyValue(x => x.WorkIff, selector: x => x is not null);
        SaveAsCmd = ReactiveCommand.CreateFromTask(
            async () =>
            {
                var path = (await ShowSaveFileDialogInteraction.Handle(Unit.Default))
                    ?.Path
                    .LocalPath;
                if (!string.IsNullOrWhiteSpace(path) && WorkIff is not null)
                {
                    iffService.Write(path, WorkIff);
                }
                return path;
            },
            canExecuteSaveAs
        );
        var saveAsCmdObs = this.WhenAnyObservable(x => x.SaveAsCmd).WhereNotNull();
        mergedPathToWorkIffChangedObs = mergedPathToWorkIffChangedObs.Merge(saveAsCmdObs);
        _pathToWorkIff = mergedPathToWorkIffChangedObs.ToProperty(this, x => x.PathToWorkIff);

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
            .Select(x => ((CarrViewModel?)x?.Content)?.JobInfos)
            .Subscribe(x =>
            {
                jobInfoSourceList.Edit(updater =>
                {
                    updater.Clear();
                    if (x is not null)
                    {
                        updater.AddRange(x);
                    }
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
        this.WhenAnyValue(x => x.WorkIff).Subscribe(_ => SelectedCareer = null);
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

        // reset preset when workiff changes
        this.WhenAnyValue(x => x.WorkIff).Subscribe(_ => SelectedPreset = null);

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
            .Select(x => $"Career Editor{(string.IsNullOrWhiteSpace(x) ? "" : $": {x}")}")
            .ToProperty(this, x => x.WindowTitle);

        AboutCmd = ReactiveCommand.Create(() => OpenUrl(AboutLink));

        _shiftLength = this.WhenAnyValue(x => x.SelectedJob)
            .Select(CalculateShiftLength)
            .ToProperty(this, x => x.ShiftLength);
        _shiftHungerDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.HungerDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftHungerDecay);
        _shiftComfortDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.ComfortDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftComfortDecay);
        _shiftHygieneDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.HygieneDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftHygieneDecay);
        _shiftBladderDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.BladderDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftBladderDecay);
        _shiftEnergyDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.EnergyDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftEnergyDecay);
        _shiftFunDecay = this.WhenAnyValue(x => x.SelectedJob, x => x.SelectedJob!.FunDecay.Value)
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftFunDecay);
        _shiftSocialDecay = this.WhenAnyValue(
                x => x.SelectedJob,
                x => x.SelectedJob!.SocialDecay.Value
            )
            .Select(x => CalculateShiftLength(x.Item1) * x.Item2)
            .ToProperty(this, x => x.ShiftSocialDecay);
    }

    private static int? CalculateShiftLength(JobInfoViewModel? vm)
    {
        if (vm is null)
        {
            return null;
        }
        var startTime = vm.StartTime.Value;
        var endTime = vm.EndTime.Value;
        if (startTime > endTime)
        {
            endTime += 24;
        }
        return endTime - startTime;
    }

    public ReadOnlyObservableCollection<ResourceViewModel> Careers => _careers;
    public ReadOnlyObservableCollection<JobInfoViewModel> Jobs => _jobs;

    private string? PathToExpansionShared => _pathToExpansionShared.Value;
    private string? PathToWorkIff => _pathToWorkIff.Value;
    private string? PathToExpansionSharedFar => _pathToExpansionSharedFar.Value;

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

    public List<IffPreset> Presets { get; } =
        new() { IffPreset.CapDecayAtNegative5, IffPreset.NoDecay };

    public IffPreset? SelectedPreset
    {
        get => _selectedPreset;
        set => this.RaiseAndSetIfChanged(ref _selectedPreset, value);
    }

    public string WindowTitle => _windowTitle.Value;

    public int? ShiftLength => _shiftLength.Value;
    public int? ShiftHungerDecay => _shiftHungerDecay.Value;
    public int? ShiftComfortDecay => _shiftComfortDecay.Value;
    public int? ShiftHygieneDecay => _shiftHygieneDecay.Value;
    public int? ShiftBladderDecay => _shiftBladderDecay.Value;
    public int? ShiftEnergyDecay => _shiftEnergyDecay.Value;
    public int? ShiftFunDecay => _shiftFunDecay.Value;
    public int? ShiftSocialDecay => _shiftSocialDecay.Value;

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

    public ReactiveCommand<Unit, Unit> SaveCmd { get; }
    public IInteraction<Unit, IStorageFile?> ShowSaveFileDialogInteraction { get; }
    public ReactiveCommand<Unit, string?> SaveAsCmd { get; }
    public ReactiveCommand<Unit, Unit> AboutCmd { get; }

    public ReactiveCommand<Unit, IffViewModel> ResetCmd { get; }
    public ReactiveCommand<Unit, string?> ShowOpenFileDialogCmd { get; init; }
    public IInteraction<Unit, IStorageFile?> ShowOpenFileDialogInteraction { get; }
    public ReactiveCommand<Unit, Unit> OpenWorkIffFolderCmd { get; }

    private string? ExtractWorkIff(string? pathToExpansionSharedFar)
    {
        if (
            string.IsNullOrWhiteSpace(pathToExpansionSharedFar)
            || !File.Exists(pathToExpansionSharedFar)
        )
        {
            return null;
        }

        var pathToExpansionShared = Path.GetDirectoryName(pathToExpansionSharedFar);
        if (
            string.IsNullOrWhiteSpace(pathToExpansionShared)
            || !Directory.Exists(pathToExpansionShared)
        )
        {
            return null;
        }

        var pathToWorkIff = Path.Combine(pathToExpansionShared, "work.iff");
        if (File.Exists(pathToWorkIff))
        {
            return pathToWorkIff;
        }

        _far.PathToFar = PathToExpansionSharedFar;
        _far.ParseFar();
        _far.Extract(
            _far.Manifest.ManifestEntries.First(x => x.Filename == "work.iff"),
            pathToExpansionShared
        );
        return pathToWorkIff;
    }

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

    // https://stackoverflow.com/a/43232486
    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
