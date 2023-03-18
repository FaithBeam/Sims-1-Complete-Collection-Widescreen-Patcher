using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Enums;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Utilities;
using Splat;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Fields

    private WrapperUtility.Wrapper _selectedWrapper;
    private Resolution? _selectedResolution;
    private string _path = "";
    private bool _isBusy;
    private double _progress;
    private readonly ObservableAsPropertyHelper<bool> _hasBackup;
    private readonly ObservableAsPropertyHelper<bool> _isValidSimsExe;
    private readonly List<string> _previouslyPatched = new();

    #endregion

    #region Constructor

    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.Path).Subscribe(x => System.Diagnostics.Debug.WriteLine(x));
        _hasBackup = this
            .WhenAnyValue(x => x.Path, x => x.IsBusy, (path, isBusy) => PatchUtility.SimsBackupExists(path))
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
                    !isBusy
            ).DistinctUntilChanged();
        var canUninstall = this
            .WhenAnyValue(x => x.IsBusy, x => x.Path, x => x.HasBackup,
                (isBusy, path, hasBackup) =>
                    !string.IsNullOrWhiteSpace(path) &&
                    !isBusy &&
                    hasBackup
            ).DistinctUntilChanged();
        PatchCommand = ReactiveCommand.CreateFromTask(OnClickedPatch, canPatch);
        UninstallCommand = ReactiveCommand.CreateFromTask(OnClickedUninstall, canUninstall);
        OpenFile = ReactiveCommand.CreateFromTask(OpenFileAsync);
        ShowOpenFileDialog = new Interaction<Unit, string>();
        var resolutionsService = Locator.Current.GetService<IResolutionsService>();
        Resolutions = new AvaloniaList<Resolution>(resolutionsService?.GetResolutions() ?? Array.Empty<Resolution>())
            { new(-1, -1) };
        SelectedResolution = Resolutions.FirstOrDefault() ?? new Resolution(1920, 1080);
        SelectedWrapper = Wrappers.FirstOrDefault();
        ShowCustomResolutionDialog = new Interaction<CustomResolutionDialogViewModel, Resolution?>();
        CustomResolutionCommand = ReactiveCommand.CreateFromTask(OpenCustomResolutionDialogAsync);
        ShowCustomYesNoDialog = new Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?>();
        ShowCustomInformationDialog = new Interaction<CustomInformationDialogViewModel, Unit>();
    }

    #endregion

    #region Commands

    public ICommand PatchCommand { get; }
    public ICommand UninstallCommand { get; }
    public ICommand OpenFile { get; }
    public Interaction<Unit, string> ShowOpenFileDialog { get; }
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

    public AvaloniaList<Resolution> Resolutions { get; }

    public Resolution? SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            if (value is { Width: -1, Height: -1 })
            {
                CustomResolutionCommand.Execute(null);
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _selectedResolution, value);
            }
        }
    }

    public AvaloniaList<WrapperUtility.Wrapper> Wrappers { get; } =
        new(WrapperUtility.Wrapper.DDrawCompat, WrapperUtility.Wrapper.DgVoodoo2, WrapperUtility.Wrapper.None);

    public WrapperUtility.Wrapper SelectedWrapper
    {
        get => _selectedWrapper;
        set => this.RaiseAndSetIfChanged(ref _selectedWrapper, value);
    }

    public double Progress
    {
        get => _progress;
        set
        {
            if (value >= 100)
            {
                this.RaiseAndSetIfChanged(ref _progress, 0);
                OpenFinishedPatchPopup();
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _progress, value);
            }
        }
    }

    #endregion

    #region Methods

    private async Task OpenCustomInformationDialogAsync(string title, string message)
    {
        var vm = new CustomInformationDialogViewModel(title, message);
        await ShowCustomInformationDialog.Handle(vm);
    }

    private async Task<YesNoDialogResponse?> OpenCustomYesNoDialogAsync(string title, string message)
    {
        var vm = new CustomYesNoDialogViewModel(title, message);
        var result = await ShowCustomYesNoDialog.Handle(vm);
        return result;
    }

    private async Task OpenCustomResolutionDialogAsync()
    {
        var vm = new CustomResolutionDialogViewModel();
        var res = await ShowCustomResolutionDialog.Handle(vm);
        if (res is { Width: > 0, Height: > 0 })
        {
            Resolutions.Insert(Resolutions.Count - 1, res);
            SelectedResolution = res;
        }
    }

    private async Task OpenFileAsync()
    {
        var fileName = await ShowOpenFileDialog.Handle(Unit.Default);
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            Path = fileName;
        }
    }

    private async Task OnClickedPatch()
    {
        IsBusy = true;
        var progress = new Progress<double>(percent => { Progress = percent; });

        if (SelectedWrapper is WrapperUtility.Wrapper.DDrawCompat)
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
            Images.Images.ModifySimsUi(Path, SelectedResolution!.Width, SelectedResolution.Height, progress));

        if (SelectedWrapper != WrapperUtility.Wrapper.None)
        {
            await Task.Run(() => WrapperUtility.RemoveWrapper(Path));
            await Task.Run(() => WrapperUtility.ExtractWrapper(SelectedWrapper, Path));
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
        _previouslyPatched.Remove(Path);
        IsBusy = false;
        await OpenCustomInformationDialogAsync("Progress", "Uninstalled");
    }

    private async Task OpenFinishedPatchPopup()
    {
        await OpenCustomInformationDialogAsync("Progress", "Patched! You may close this application now.");
    }

    #endregion
}
