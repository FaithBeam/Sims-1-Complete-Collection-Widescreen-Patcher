using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
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
        _hasBackup = this.
            WhenAnyValue(x => x.Path, x => x.IsBusy, (path, isBusy) => PatchUtility.SimsBackupExists(path))
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
        Resolutions = new AvaloniaList<Resolution>(resolutionsService?.GetResolutions() ?? Array.Empty<Resolution>()) { new(-1, -1) };
        SelectedResolution = Resolutions.FirstOrDefault() ?? new Resolution(1920, 1080);
        SelectedWrapper = Wrappers.FirstOrDefault();
        ShowCustomResolutionDialog = new Interaction<CustomResolutionDialogViewModel, Resolution?>();
        CustomResolutionCommand = ReactiveCommand.CreateFromTask(OpenCustomResolutionDialogAsync);
    }

    #endregion

    #region Commands

    public ICommand PatchCommand { get; }
    public ICommand UninstallCommand { get; }
    public ICommand OpenFile { get; }
    public Interaction<Unit, string> ShowOpenFileDialog { get; }
    public ICommand CustomResolutionCommand { get; }
    public Interaction<CustomResolutionDialogViewModel, Resolution?> ShowCustomResolutionDialog { get; }

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
        new(WrapperUtility.Wrapper.None, WrapperUtility.Wrapper.DgVoodoo2, WrapperUtility.Wrapper.DDrawCompat);

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
        await Task.Run(() => PatchUtility.Patch(Path, SelectedResolution!.Width, SelectedResolution.Height));
        await Task.Run(() => Images.Images.ModifySimsUi(Path, SelectedResolution!.Width, SelectedResolution.Height, progress));

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
        await Task.Run(() => UninstallUtility.Uninstall(Path));
        _previouslyPatched.Remove(Path);
        IsBusy = false;
        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Progress", "Uninstalled!", icon: Icon.Info);
        await messageBoxStandardWindow.Show();
    }

    private static void OpenFinishedPatchPopup()
    {
        var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Progress", "Patched! You may close this application now.", icon: Icon.Info);
        messageBoxStandardWindow.Show();
    }
    
    #endregion
}