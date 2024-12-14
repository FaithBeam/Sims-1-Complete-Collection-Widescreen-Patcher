using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Enums;
using Sims1WidescreenPatcher.Core.Events;
using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using Sims1WidescreenPatcher.Core.Validations;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public interface IMainTabViewModel
{
    ReactiveCommand<Unit, Unit>? PatchCommand { get; }
    ReactiveCommand<Unit, Unit>? UninstallCommand { get; }
    ReactiveCommand<Unit, Unit>? OpenFile { get; }
    Interaction<Unit, IStorageFile?>? ShowOpenFileDialog { get; }
    ReactiveCommand<Unit, Unit>? CustomResolutionCommand { get; }
    Interaction<ICustomResolutionDialogViewModel?, Resolution?>? ShowCustomResolutionDialog { get; }
    Interaction<CustomYesNoDialogViewModel?, YesNoDialogResponse?>? ShowCustomYesNoDialog { get; }
    Interaction<CustomInformationDialogViewModel, Unit>? ShowCustomInformationDialog { get; }
    string Path { get; set; }
    AspectRatio? SelectedAspectRatio { get; set; }
    ReadOnlyObservableCollection<AspectRatio>? AspectRatios { get; }
    ReadOnlyObservableCollection<Resolution>? FilteredResolutions { get; }
    Resolution? SelectedResolution { get; set; }
    AvaloniaList<IWrapper>? Wrappers { get; }
    int SelectedWrapperIndex { get; set; }
    double Progress { get; }
    public ICheckboxViewModel? ResolutionsColoredCbVm { get; }
    public ICheckboxViewModel? SortByAspectRatioCbVm { get; }
}

public class MainTabViewModel : ViewModelBase, IMainTabViewModel
{
    #region Fields

    private readonly CustomYesNoDialogViewModel _customYesNoDialogViewModel;
    private readonly ICustomResolutionDialogViewModel _customResolutionDialogViewModel;
    private int _selectedWrapperIndex;
    private Resolution? _selectedResolution;
    private Resolution? _previousResolution;
    private AspectRatio? _selectedSelectedAspectRatio;
    private string _path = "";
    private bool _isBusy;
    private readonly ObservableAsPropertyHelper<bool>? _hasBackup;
    private readonly ObservableAsPropertyHelper<bool>? _isValidSimsExe;
    private readonly List<string>? _previouslyPatched;
    private readonly ObservableAsPropertyHelper<double> _progressPct;
    private readonly ObservableAsPropertyHelper<string?> _progressStatus;
    private readonly ObservableAsPropertyHelper<string?> _progressStatus2;
    private readonly SourceList<Resolution> _resolutionSource;
    private readonly IProgressService _progressService;
    private readonly IWrapperService _wrapperService;
    private readonly ReadOnlyObservableCollection<Resolution> _filteredResolutions;
    private readonly ReadOnlyObservableCollection<AspectRatio> _aspectRatios;
    private readonly IResolutionPatchService _resolutionPatchService;
    private readonly IUninstallService _uninstallService;
    private readonly IImagesService _imagesService;
    private readonly ObservableAsPropertyHelper<string?> _patchButtonToolTipTxt;
    private IAppState? AppState { get; }

    private const string IncompatibleSimsExeTxt =
        "Your Sims.exe is not able to be patched.\nYou need to be using a cracked/nocd Sims.exe that has not been patched to a custom resolution.";

    #endregion

    #region Constructor

    public MainTabViewModel(
        IResolutionsService resolutionsService,
        CustomYesNoDialogViewModel customYesNoDialogViewModel,
        ICustomResolutionDialogViewModel customResolutionDialogViewModel,
        IFindSimsPathService findSimsPathService,
        CheckboxViewModelFactory ucVmFactory,
        IAppState appState,
        IResolutionPatchService resolutionPatchService,
        IUninstallService uninstallService,
        IImagesService imagesService,
        IProgressService progressService,
        IWrapperService wrapperService
    )
    {
        _previouslyPatched = new List<string>();
        _resolutionSource = new SourceList<Resolution>();
        AppState = appState;
        _progressService = progressService;
        _wrapperService = wrapperService;
        _resolutionPatchService = resolutionPatchService;
        _uninstallService = uninstallService;
        _imagesService = imagesService;
        ResolutionsColoredCbVm = (CheckboxViewModel)ucVmFactory.Create("Color Code");
        ResolutionsColoredCbVm.Checked = true;
        ResolutionsColoredCbVm.ToolTipText = "Color code the resolutions by their aspect ratio";
        SortByAspectRatioCbVm = (CheckboxViewModel)ucVmFactory.Create("Sort by Aspect Ratio");
        SortByAspectRatioCbVm.ToolTipText = "Sort the resolutions by their aspect ratio";
        _customYesNoDialogViewModel = customYesNoDialogViewModel;
        _customResolutionDialogViewModel = customResolutionDialogViewModel;

        OpenFile = ReactiveCommand.CreateFromTask(OpenFileAsync);
        ShowOpenFileDialog = new Interaction<Unit, IStorageFile?>();

        SelectedWrapperIndex = 0;
        SelectedAspectRatio = null;
        ShowCustomResolutionDialog =
            new Interaction<ICustomResolutionDialogViewModel?, Resolution?>();
        CustomResolutionCommand = ReactiveCommand.CreateFromTask(OpenCustomResolutionDialogAsync);
        ShowCustomYesNoDialog =
            new Interaction<CustomYesNoDialogViewModel?, YesNoDialogResponse?>();
        ShowCustomInformationDialog = new Interaction<CustomInformationDialogViewModel, Unit>();

        ShowBadSimsExeInfoDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            await OpenCustomInformationDialogAsync("Information", IncompatibleSimsExeTxt);
        });

        var resolutionFilter = this.WhenAnyValue(x => x.SelectedAspectRatio)
            .Select(CreateResolutionPredicate);
        var resolutionSort = this.WhenAnyValue(x => x.SortByAspectRatioCbVm!.Checked)
            .Select(x =>
                x
                    ? SortExpressionComparer<Resolution>
                        .Ascending(r => r.AspectRatio.Numerator)
                        .ThenByAscending(r => r.AspectRatio.Denominator)
                        .ThenByAscending(r => r.Width)
                        .ThenByAscending(r => r.Height)
                    : SortExpressionComparer<Resolution>
                        .Ascending(r => r.Width)
                        .ThenByAscending(r => r.Height)
            );

        if (_resolutionSource.Count < 1)
        {
            _resolutionSource.AddRange(resolutionsService.GetResolutions());
        }

        if (string.IsNullOrWhiteSpace(Path))
        {
            Path = findSimsPathService.FindSimsPath();
        }

        this.WhenAnyValue(x => x.Path)
            .Subscribe(x =>
            {
                if (AppState != null)
                {
                    AppState.SimsExePath = x;
                }
            });
        this.WhenAnyValue(x => x.SelectedResolution)
            .Subscribe(x =>
            {
                if (AppState != null)
                {
                    AppState.Resolution = x;
                }
            });
        _hasBackup = this.WhenAnyValue(x => x.Path, x => x.IsBusy)
            .Select(_ => _resolutionPatchService.BackupExists())
            .ToProperty(this, x => x.HasBackup, deferSubscription: true);
        _isValidSimsExe = this.WhenAnyValue(x => x.Path)
            .Select(_ => _resolutionPatchService.CanPatchResolution())
            .ToProperty(this, x => x.IsValidSimsExe, deferSubscription: true);

        _resolutionSource
            .Connect()
            .Filter(resolutionFilter)
            .Sort(resolutionSort)
            .Bind(out _filteredResolutions)
            .Subscribe(GetNewSelectedResolution);
        _resolutionSource
            .Connect()
            .DistinctValues(x => x.AspectRatio)
            .Sort(Comparer<AspectRatio>.Default)
            .Bind(out _aspectRatios)
            .Subscribe();

        var canPatch = this.WhenAnyValue(
                x => x.IsBusy,
                x => x.Path,
                x => x.IsValidSimsExe,
                x => x.HasBackup,
                (isBusy, path, validSimsExe, hasBackup) =>
                    !string.IsNullOrWhiteSpace(path)
                    && !hasBackup
                    && validSimsExe
                    && !_previouslyPatched.Contains(path)
                    && !isBusy
            )
            .DistinctUntilChanged();
        var canUninstall = this.WhenAnyValue(
                x => x.IsBusy,
                x => x.Path,
                x => x.HasBackup,
                (isBusy, path, hasBackup) =>
                    !string.IsNullOrWhiteSpace(path) && !isBusy && hasBackup
            )
            .DistinctUntilChanged();
        PatchCommand = ReactiveCommand.CreateFromTask(OnClickedPatch, canPatch);
        UninstallCommand = ReactiveCommand.CreateFromTask(OnClickedUninstall, canUninstall);

        this.WhenAnyValue(
                x => x.IsValidSimsExe,
                x => x.Path,
                x => x.HasBackup,
                selector: (validExe, path, hasBackup) =>
                    !validExe && !hasBackup && !string.IsNullOrWhiteSpace(path)
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            // this is terrible. Without this, the command is invoked before the view has a chance to bind an
            // interaction for ShowCustomInformationDialog, causing the app to crash
            .DelaySubscription(TimeSpan.FromSeconds(1))
            .Where(x => x)
            .Select(_ => Unit.Default)
            .InvokeCommand(ShowBadSimsExeInfoDialog);
        _patchButtonToolTipTxt = this.WhenAnyValue(
                x => x.IsValidSimsExe,
                x => x.Path,
                x => x.HasBackup,
                selector: (validExe, path, hasBackup) =>
                    !validExe && !hasBackup && !string.IsNullOrWhiteSpace(path)
            )
            .ObserveOn(RxApp.MainThreadScheduler)
            .DelaySubscription(TimeSpan.FromSeconds(1))
            .Where(x => x)
            .Select(_ => IncompatibleSimsExeTxt)
            .ToProperty(this, x => x.PatchButtonToolTipTxt);

        _progressPct = progressService
            .NewProgressObservable.ObserveOn(RxApp.MainThreadScheduler)
            .Select(x => x.Progress)
            .ToProperty(this, x => x.Progress);
        _progressStatus = progressService
            .NewProgressObservable.ObserveOn(RxApp.MainThreadScheduler)
            .Select(x => x.Status)
            .ToProperty(this, x => x.ProgressStatus);
        _progressStatus2 = progressService
            .NewProgressObservable.ObserveOn(RxApp.MainThreadScheduler)
            .Select(x => x.Status2)
            .ToProperty(this, x => x.ProgressStatus2);

        if (Wrappers is null || !Wrappers.Any())
        {
            Wrappers = new AvaloniaList<IWrapper>(_wrapperService.GetWrappers());
        }

        SelectedResolution ??= FilteredResolutions.First();
    }

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> ShowBadSimsExeInfoDialog { get; }
    public ReactiveCommand<Unit, Unit> PatchCommand { get; }
    public ReactiveCommand<Unit, Unit> UninstallCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenFile { get; }
    public Interaction<Unit, IStorageFile?> ShowOpenFileDialog { get; }
    public ReactiveCommand<Unit, Unit> CustomResolutionCommand { get; }

    public Interaction<
        ICustomResolutionDialogViewModel?,
        Resolution?
    > ShowCustomResolutionDialog { get; }
    public Interaction<
        CustomYesNoDialogViewModel?,
        YesNoDialogResponse?
    > ShowCustomYesNoDialog { get; }

    public Interaction<CustomInformationDialogViewModel, Unit> ShowCustomInformationDialog { get; }

    #endregion

    #region Properties

    public string? PatchButtonToolTipTxt => _patchButtonToolTipTxt.Value;

    [RequiredAlt]
    [FileExists]
    public string Path
    {
        get => _path;
        set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    private bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    private bool HasBackup => _hasBackup?.Value ?? false;
    private bool IsValidSimsExe => _isValidSimsExe?.Value ?? false;

    public AspectRatio? SelectedAspectRatio
    {
        get => _selectedSelectedAspectRatio;
        set => this.RaiseAndSetIfChanged(ref _selectedSelectedAspectRatio, value);
    }

    public ICheckboxViewModel? ResolutionsColoredCbVm { get; }

    public ICheckboxViewModel? SortByAspectRatioCbVm { get; }

    public ReadOnlyObservableCollection<AspectRatio> AspectRatios => _aspectRatios;

    public ReadOnlyObservableCollection<Resolution> FilteredResolutions => _filteredResolutions;

    public Resolution? SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            if (value is null)
            {
                _previousResolution = _selectedResolution;
            }

            this.RaiseAndSetIfChanged(ref _selectedResolution, value);
        }
    }

    public AvaloniaList<IWrapper> Wrappers { get; }

    public int SelectedWrapperIndex
    {
        get => _selectedWrapperIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedWrapperIndex, value);
    }

    public double Progress => _progressPct.Value;

    public string? ProgressStatus => _progressStatus.Value;

    public string? ProgressStatus2 => _progressStatus2.Value;

    #endregion

    #region Methods

    private void GetNewSelectedResolution(IChangeSet<Resolution> changeSet)
    {
        if (changeSet.Moves > 0)
        {
            SelectedResolution ??= _previousResolution;
            return;
        }

        foreach (var change in changeSet)
        {
            switch (change.Reason)
            {
                case ListChangeReason.Add:
                    SelectedResolution = change.Item.Current;
                    break;
                case ListChangeReason.AddRange:
                    if (
                        FilteredResolutions != null
                        && FilteredResolutions.All(x => x != SelectedResolution)
                    )
                    {
                        SelectedResolution = change.Range.First();
                        return;
                    }

                    break;
                case ListChangeReason.Replace:
                    break;
                case ListChangeReason.Remove:
                    break;
                case ListChangeReason.RemoveRange:
                    SelectedResolution = FilteredResolutions?.First();
                    return;
                case ListChangeReason.Refresh:
                    break;
                case ListChangeReason.Moved:
                    break;
                case ListChangeReason.Clear:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private Func<Resolution, bool> CreateResolutionPredicate(AspectRatio? ar)
    {
        if (ar is null)
        {
            return _ => true;
        }

        return resolution => resolution.AspectRatio == ar;
    }

    private async Task OpenCustomInformationDialogAsync(string title, string message)
    {
        var vm = new CustomInformationDialogViewModel(title, message);
        await ShowCustomInformationDialog.Handle(vm);
    }

    private async Task<YesNoDialogResponse?> OpenCustomYesNoDialogAsync(
        string title,
        string message
    )
    {
        _customYesNoDialogViewModel.Title = title;
        _customYesNoDialogViewModel.Message = message;
        var result = await ShowCustomYesNoDialog.Handle(_customYesNoDialogViewModel);
        return result;
    }

    private async Task OpenCustomResolutionDialogAsync()
    {
        var res = await ShowCustomResolutionDialog.Handle(_customResolutionDialogViewModel);
        if (res is null)
        {
            return;
        }

        _resolutionSource.Add(res);
    }

    private async Task OpenFileAsync()
    {
        var storageFile = await ShowOpenFileDialog.Handle(Unit.Default);
        if (storageFile is not null)
        {
            Path = storageFile.Path.LocalPath;
        }
    }

    private async Task OnClickedPatch()
    {
        IsBusy = true;

        var selectedWrapper = Wrappers[SelectedWrapperIndex];

        if (selectedWrapper is DDrawCompatWrapper { Version: "0.5.4" })
        {
            var result = await OpenCustomYesNoDialogAsync(
                "DDrawCompat Settings",
                "Enable borderless fullscreen mode?\n(Choosing \"no\" may cause issues on variable refresh rate displays.)"
            );
            if (result is not null && result.Result)
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(
                    Path,
                    DDrawCompatEnums.BorderlessFullscreen
                );
            }
            else
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(
                    Path,
                    DDrawCompatEnums.ExclusiveFullscreen
                );
            }
        }

        await Task.Run(() => _resolutionPatchService.CreateBackup());
        await Task.Run(() => _resolutionPatchService.EditSimsExe());
        await Task.Run(() => _imagesService.Install());

        if (selectedWrapper is not NoneWrapper)
        {
            await Task.Run(() => _wrapperService.Uninstall());
            await Task.Run(() => _wrapperService.Install(selectedWrapper));
        }

        _previouslyPatched?.Add(Path);

        await OpenCustomInformationDialogAsync(
            "Progress",
            "Patched! You may close this application now."
        );
        _progressService.UpdateProgress(0.0);

        IsBusy = false;
    }

    private async Task OnClickedUninstall()
    {
        IsBusy = true;
        var dDrawSettingsPath = DDrawCompatSettingsService.DDrawCompatSettingsExist(Path);
        if (!string.IsNullOrWhiteSpace(dDrawSettingsPath))
        {
            var result = await OpenCustomYesNoDialogAsync(
                "Uninstall",
                $"DDrawCompat settings were found at:\n{dDrawSettingsPath}\n\nDo you wish to remove them?"
            );
            if (result is not null && result.Result)
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(
                    Path,
                    DDrawCompatEnums.BorderlessFullscreen
                );
            }

            if (result is not null && result.Result)
            {
                DDrawCompatSettingsService.Remove(dDrawSettingsPath);
            }
        }

        await Task.Run(() => _uninstallService.Uninstall());
        _previouslyPatched?.Remove(Path);
        IsBusy = false;
        await OpenCustomInformationDialogAsync("Progress", "Uninstalled");
    }

    #endregion
}
