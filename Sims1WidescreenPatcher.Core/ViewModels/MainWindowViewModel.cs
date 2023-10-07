using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Enums;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Utilities;
using Sims1WidescreenPatcher.Utilities.Models;
using Sims1WidescreenPatcher.Utilities.Services;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    #region Fields

    private readonly CustomYesNoDialogViewModel _customYesNoDialogViewModel;
    private readonly CustomResolutionDialogViewModel _customResolutionDialogViewModel;
    private int _selectedWrapperIndex;
    private Resolution? _selectedResolution;
    private AspectRatio? _selectedSelectedAspectRatio;
    private string _path = "";
    private bool _isBusy;
    private bool _isResolutionsColored;
    private bool _sortByAspectRatio;
    private readonly ObservableAsPropertyHelper<bool> _hasBackup;
    private readonly ObservableAsPropertyHelper<bool> _isValidSimsExe;
    private readonly List<string> _previouslyPatched = new();
    private readonly IProgressService _progressService;
    private readonly ObservableAsPropertyHelper<double> _progress;
    private readonly SourceList<Resolution> _resolutionSource = new();
    private readonly ReadOnlyObservableCollection<Resolution> _filteredResolutions;
    private readonly ReadOnlyObservableCollection<AspectRatio> _aspectRatios;

    #endregion

    #region Constructor

    public MainWindowViewModel(IResolutionsService resolutionsService,
        CustomYesNoDialogViewModel customYesNoDialogViewModel,
        CustomResolutionDialogViewModel customResolutionDialogViewModel,
        IProgressService progressService,
        IFindSimsPathService findSimsPathService)
    {
        _progressService = progressService;
        _customYesNoDialogViewModel = customYesNoDialogViewModel;
        _customResolutionDialogViewModel = customResolutionDialogViewModel;
        this.WhenAnyValue(x => x.Path)
            .Subscribe(x => System.Diagnostics.Debug.WriteLine(x));
        _hasBackup = this
            .WhenAnyValue(x => x.Path, x => x.IsBusy, (path, _) => PatchUtility.SimsBackupExists(path))
            .ToProperty(this, x => x.HasBackup, deferSubscription: true);
        _isValidSimsExe = this
            .WhenAnyValue(x => x.Path, PatchUtility.IsValidSims)
            .ToProperty(this, x => x.IsValidSimsExe, deferSubscription: true);
        var canPatch = this
            .WhenAnyValue(x => x.IsBusy, x => x.Path, x => x.IsValidSimsExe, x => x.HasBackup,
                (isBusy, path, validSimsExe, hasBackup) =>
                    !string.IsNullOrWhiteSpace(path) &&
                    !hasBackup &&
                    validSimsExe &&
                    !_previouslyPatched.Contains(path) &&
                    !isBusy)
            .DistinctUntilChanged();
        var canUninstall = this
            .WhenAnyValue(x => x.IsBusy, x => x.Path, x => x.HasBackup,
                (isBusy, path, hasBackup) =>
                    !string.IsNullOrWhiteSpace(path) &&
                    !isBusy &&
                    hasBackup)
            .DistinctUntilChanged();
        PatchCommand = ReactiveCommand.CreateFromTask(OnClickedPatch, canPatch);
        UninstallCommand = ReactiveCommand.CreateFromTask(OnClickedUninstall, canUninstall);
        OpenFile = ReactiveCommand.CreateFromTask(OpenFileAsync);
        ShowOpenFileDialog = new Interaction<Unit, IStorageFile?>();
        var resolutionFilter = this
            .WhenAnyValue(x => x.SelectedAspectRatio)
            .Select(CreateResolutionPredicate);
        var resolutionSort = this
            .WhenAnyValue(x => x.SortByAspectRatio)
            .Select(x => x
                ? SortExpressionComparer<Resolution>
                    .Ascending(r => r.AspectRatio.Numerator)
                    .ThenByAscending(r => r.AspectRatio.Denominator)
                    .ThenByAscending(r => r.Width)
                    .ThenByAscending(r => r.Height)
                : SortExpressionComparer<Resolution>
                    .Ascending(r => r.Width)
                    .ThenByAscending(r => r.Height));
        _resolutionSource
            .Connect()
            .Filter(resolutionFilter)
            .Sort(resolutionSort)
            .Bind(out _filteredResolutions)
            .Subscribe(x => SelectedResolution = x.Last().Item.Current);
        _resolutionSource
            .Connect()
            .DistinctValues(x => x.AspectRatio)
            .Sort(Comparer<AspectRatio>.Default)
            .Bind(out _aspectRatios)
            .Subscribe();
        
        _resolutionSource.AddRange(resolutionsService.GetResolutions());
        SelectedResolution = FilteredResolutions.First();
        SelectedWrapperIndex = 0;
        SelectedAspectRatio = null;
        ShowCustomResolutionDialog = new Interaction<CustomResolutionDialogViewModel, Resolution?>();
        CustomResolutionCommand = ReactiveCommand.CreateFromTask(OpenCustomResolutionDialogAsync);
        ShowCustomYesNoDialog = new Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?>();
        ShowCustomInformationDialog = new Interaction<CustomInformationDialogViewModel, Unit>();
        Path = findSimsPathService.FindSimsPath();
        
        var progressPct = Observable
            .FromEventPattern<NewProgressEventArgs>(_progressService, "NewProgressEventHandler");
        progressPct
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async e =>
            {
                if (e.EventArgs.Progress < 100) return;
                await OpenCustomInformationDialogAsync("Progress", "Patched! You may close this application now.");
                _progressService.UpdateProgress(0.0);
            });
        _progress = progressPct
            .Select(x => x.EventArgs.Progress)
            .ToProperty(this, x => x.Progress);
    }

    #endregion

    #region Commands

    public ICommand PatchCommand { get; }
    public ICommand UninstallCommand { get; }
    public ICommand OpenFile { get; }
    public Interaction<Unit, IStorageFile?> ShowOpenFileDialog { get; }
    public ICommand CustomResolutionCommand { get; }
    public Interaction<CustomResolutionDialogViewModel, Resolution?> ShowCustomResolutionDialog { get; }
    public Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?> ShowCustomYesNoDialog { get; }
    public Interaction<CustomInformationDialogViewModel, Unit> ShowCustomInformationDialog { get; }

    #endregion

    #region Properties

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

    private bool HasBackup => _hasBackup.Value;
    private bool IsValidSimsExe => _isValidSimsExe.Value;

    public AspectRatio? SelectedAspectRatio
    {
        get => _selectedSelectedAspectRatio;
        set => this.RaiseAndSetIfChanged(ref _selectedSelectedAspectRatio, value);
    }

    public bool SortByAspectRatio
    {
        get => _sortByAspectRatio;
        set => this.RaiseAndSetIfChanged(ref _sortByAspectRatio, value);
    }

    public bool IsResolutionsColored
    {
        get => _isResolutionsColored;
        set => this.RaiseAndSetIfChanged(ref _isResolutionsColored, value);
    }

    public ReadOnlyObservableCollection<AspectRatio> AspectRatios => _aspectRatios;

    public ReadOnlyObservableCollection<Resolution> FilteredResolutions => _filteredResolutions;

    public Resolution? SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            if (value is null)
            {
                this.RaiseAndSetIfChanged(ref _selectedResolution, _filteredResolutions.FirstOrDefault());
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _selectedResolution, value);
            }
        }
    }

    public AvaloniaList<IWrapper> Wrappers => new(WrapperUtility.GetWrappers());

    public int SelectedWrapperIndex
    {
        get => _selectedWrapperIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedWrapperIndex, value);
    }

    public double Progress => _progress.Value;

    #endregion

    #region Methods

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

    private async Task<YesNoDialogResponse?> OpenCustomYesNoDialogAsync(string title, string message)
    {
        _customYesNoDialogViewModel.Title = title;
        _customYesNoDialogViewModel.Message = message;
        var result = await ShowCustomYesNoDialog.Handle(_customYesNoDialogViewModel);
        return result;
    }

    private async Task OpenCustomResolutionDialogAsync()
    {
        var res = await ShowCustomResolutionDialog.Handle(_customResolutionDialogViewModel);
        if (res is { Width: > 0, Height: > 0 })
        {
            _resolutionSource.Add(res);
            SelectedResolution = res;
        }
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

        if (selectedWrapper is DDrawCompatWrapper { Version: "0.4.0" })
        {
            var result = await OpenCustomYesNoDialogAsync("DDrawCompat Settings",
                "Enable borderless fullscreen mode?\n(Choosing \"no\" may cause issues on variable refresh rate displays.)");
            if (result is not null && result.Result)
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(Path,
                    DDrawCompatEnums.BorderlessFullscreen);
            }
            else
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(Path,
                    DDrawCompatEnums.ExclusiveFullscreen);
            }
        }

        await Task.Run(() => PatchUtility.Patch(Path, SelectedResolution!.Width, SelectedResolution.Height));
        await Task.Run(() =>
            Images.Images.ModifySimsUi(Path, SelectedResolution!.Width, SelectedResolution.Height, _progressService));

        if (selectedWrapper is not NoneWrapper)
        {
            await Task.Run(() => WrapperUtility.RemoveWrapper(Path));
            await Task.Run(() => WrapperUtility.ExtractWrapper(selectedWrapper, Path));
        }

        _previouslyPatched.Add(Path);
        IsBusy = false;
    }

    private async Task OnClickedUninstall()
    {
        IsBusy = true;
        var ddrawSettingsPath = CheckDDrawCompatIniService.DDrawCompatSettingsExist(Path);
        if (!string.IsNullOrWhiteSpace(ddrawSettingsPath))
        {
            var result = await OpenCustomYesNoDialogAsync("Uninstall",
                $"DDrawCompat settings were found at:\n{ddrawSettingsPath}\n\nDo you wish to remove them?");
            if (result is not null && result.Result)
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(Path,
                    DDrawCompatEnums.BorderlessFullscreen);
            }

            if (result is not null && result.Result)
            {
                RemoveDDrawCompatSettingsService.Remove(ddrawSettingsPath);
            }
        }

        await Task.Run(() => UninstallUtility.Uninstall(Path));
        await Task.Run(() => Images.Images.RemoveGraphics(Path));
        _previouslyPatched.Remove(Path);
        IsBusy = false;
        await OpenCustomInformationDialogAsync("Progress", "Uninstalled");
    }

    #endregion
}