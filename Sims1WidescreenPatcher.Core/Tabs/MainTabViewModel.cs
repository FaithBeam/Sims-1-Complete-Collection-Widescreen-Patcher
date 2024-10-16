﻿using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Enums;
using Sims1WidescreenPatcher.Core.Factories;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.Services.Interfaces;
using Sims1WidescreenPatcher.Core.Validations;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

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
    private readonly ObservableAsPropertyHelper<bool> _hasBackup;
    private readonly ObservableAsPropertyHelper<bool> _isValidSimsExe;
    private readonly List<string> _previouslyPatched = new() { };
    private double _progressPct;
    private string? _progressStatus;
    private string? _progressStatus2;
    private readonly SourceList<Resolution> _resolutionSource = new();
    private readonly IProgressService _progressService;
    private readonly IWrapperService _wrapperService;
    private readonly ReadOnlyObservableCollection<Resolution> _filteredResolutions;
    private readonly ReadOnlyObservableCollection<AspectRatio> _aspectRatios;
    private readonly IResolutionPatchService _resolutionPatchService;
    private readonly IUninstallService _uninstallService;
    private readonly IImagesService _imagesService;
    private IAppState AppState { get; }

    #endregion

    #region Constructor

    public MainTabViewModel(IResolutionsService resolutionsService,
        CustomYesNoDialogViewModel customYesNoDialogViewModel,
        ICustomResolutionDialogViewModel customResolutionDialogViewModel,
        IFindSimsPathService findSimsPathService,
        CheckboxViewModelFactory ucVmFactory, IAppState appState, IResolutionPatchService resolutionPatchService,
        IUninstallService uninstallService, IImagesService imagesService, IProgressService progressService,
        IWrapperService wrapperService)
    {
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
        this
            .WhenAnyValue(x => x.Path)
            .Subscribe(x => AppState.SimsExePath = x);
        this
            .WhenAnyValue(x => x.SelectedResolution)
            .Subscribe(x => AppState.Resolution = x);
        _hasBackup = this
            .WhenAnyValue(x => x.Path, x => x.IsBusy)
            .Select(_ => _resolutionPatchService.BackupExists())
            .ToProperty(this, x => x.HasBackup, deferSubscription: true);
        _isValidSimsExe = this
            .WhenAnyValue(x => x.Path)
            .Select(_ => _resolutionPatchService.CanPatchResolution())
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
            .WhenAnyValue(x => x.SortByAspectRatioCbVm.Checked)
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
            .Subscribe(GetNewSelectedResolution);
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
        ShowCustomResolutionDialog = new Interaction<ICustomResolutionDialogViewModel, Resolution?>();
        CustomResolutionCommand = ReactiveCommand.CreateFromTask(OpenCustomResolutionDialogAsync);
        ShowCustomYesNoDialog = new Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?>();
        ShowCustomInformationDialog = new Interaction<CustomInformationDialogViewModel, Unit>();
        Path = findSimsPathService.FindSimsPath();

        progressService.NewProgressObservable.Subscribe(x =>
        {
            Progress = x.Progress;
            ProgressStatus = x.Status;
            ProgressStatus2 = x.Status2;
        });
    }

    #endregion

    #region Commands

    public ICommand PatchCommand { get; }
    public ICommand UninstallCommand { get; }
    public ICommand OpenFile { get; }
    public Interaction<Unit, IStorageFile?> ShowOpenFileDialog { get; }
    public ICommand CustomResolutionCommand { get; }
    public Interaction<ICustomResolutionDialogViewModel, Resolution?> ShowCustomResolutionDialog { get; }
    public Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?> ShowCustomYesNoDialog { get; }
    public Interaction<CustomInformationDialogViewModel, Unit> ShowCustomInformationDialog { get; }

    #endregion

    #region Properties

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

    private bool HasBackup => _hasBackup.Value;
    private bool IsValidSimsExe => _isValidSimsExe.Value;

    public AspectRatio? SelectedAspectRatio
    {
        get => _selectedSelectedAspectRatio;
        set => this.RaiseAndSetIfChanged(ref _selectedSelectedAspectRatio, value);
    }

    public ICheckboxViewModel ResolutionsColoredCbVm { get; set; }

    public ICheckboxViewModel SortByAspectRatioCbVm { get; set; }

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

    public AvaloniaList<IWrapper> Wrappers => new(_wrapperService.GetWrappers());

    public int SelectedWrapperIndex
    {
        get => _selectedWrapperIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedWrapperIndex, value);
    }

    public double Progress
    {
        get => _progressPct;
        set => this.RaiseAndSetIfChanged(ref _progressPct, value);
    }

    public string? ProgressStatus
    {
        get => _progressStatus;
        set => this.RaiseAndSetIfChanged(ref _progressStatus, value);
    }

    public string? ProgressStatus2
    {
        get => _progressStatus2;
        set => this.RaiseAndSetIfChanged(ref _progressStatus2, value);
    }

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
                    if (FilteredResolutions.All(x => x != SelectedResolution))
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
                    SelectedResolution = FilteredResolutions.First();
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

        await Task.Run(() => _resolutionPatchService.CreateBackup());
        await Task.Run(() => _resolutionPatchService.EditSimsExe());
        await Task.Run(() => _imagesService.Install());

        if (selectedWrapper is not NoneWrapper)
        {
            await Task.Run(() => _wrapperService.Uninstall());
            await Task.Run(() => _wrapperService.Install(selectedWrapper));
        }

        _previouslyPatched.Add(Path);

        await OpenCustomInformationDialogAsync("Progress", "Patched! You may close this application now.");
        _progressService.UpdateProgress(0.0);

        IsBusy = false;
    }

    private async Task OnClickedUninstall()
    {
        IsBusy = true;
        var dDrawSettingsPath = CheckDDrawCompatIniService.DDrawCompatSettingsExist(Path);
        if (!string.IsNullOrWhiteSpace(dDrawSettingsPath))
        {
            var result = await OpenCustomYesNoDialogAsync("Uninstall",
                $"DDrawCompat settings were found at:\n{dDrawSettingsPath}\n\nDo you wish to remove them?");
            if (result is not null && result.Result)
            {
                await DDrawCompatSettingsService.CreateDDrawCompatSettingsFile(Path,
                    DDrawCompatEnums.BorderlessFullscreen);
            }

            if (result is not null && result.Result)
            {
                RemoveDDrawCompatSettingsService.Remove(dDrawSettingsPath);
            }
        }

        await Task.Run(() => _uninstallService.Uninstall());
        _previouslyPatched.Remove(Path);
        IsBusy = false;
        await OpenCustomInformationDialogAsync("Progress", "Uninstalled");
    }

    #endregion
}