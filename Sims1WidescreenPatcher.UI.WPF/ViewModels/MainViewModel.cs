using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;
using Sims1WidescreenPatcher.Far;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Patch;
using Sims1WidescreenPatcher.Resolutions;
using Sims1WidescreenPatcher.UI.WPF.Services;
using Sims1WidescreenPatcher.Uninstall;
using Sims1WidescreenPatcher.Wrappers;
using Sims1WidescreenPatcher.Wrappers.Models;

namespace Sims1WidescreenPatcher.UI.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private string _path;
        private double _progress;
        private bool _isBusy;
        private bool _resolutionComboBoxEnabled = true;
        private bool _wrapperComboBoxEnabled = true;
        private bool _browseButtonEnabled = true;
        private bool _invalidExeDialogShown = false;
        private Resolution _selectedResolution;
        private Wrapper _selectedWrapper;
        private readonly IDialogService _dialogService;
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly Regex _resolutionRegex = new Regex(@"^\d+x\d+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly List<string> _previouslyPatchedList = new List<string>();

        #endregion

        #region Properties

        public List<Resolution> Resolutions { get; set; }
        public Resolution SelectedResolution
        {
            get => _selectedResolution;
            set
            {
                SetProperty(ref _selectedResolution, value);
                PatchCommand.RaiseCanExecuteChanged();
            } 
        }
        public string Path
        {
            get => _path;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                _ = SetProperty(ref _path, value);
                _invalidExeDialogShown = false;
                PatchCommand.RaiseCanExecuteChanged();
                UninstallCommand.RaiseCanExecuteChanged();
            }
        }
        public double Progress
        {
            get => _progress;
            set
            {
                _ = SetProperty(ref _progress, value);
                if (_progress >= 100)
                {
                    OpenFinishedPatchPopup();
                }
            }
        }
        public IEnumerable<Wrapper> Wrappers { get; } = new[]
        {
            Wrapper.None,
            Wrapper.DgVoodoo2,
            Wrapper.DDrawCompat
        };
        public Wrapper SelectedWrapper
        {
            get => _selectedWrapper;
            set => SetProperty(ref _selectedWrapper, value);
        }
        public bool ResolutionComboBoxEnabled
        {
            get => _resolutionComboBoxEnabled;
            set => SetProperty(ref _resolutionComboBoxEnabled, value);
        }
        public bool WrapperComboBoxEnabled
        {
            get => _wrapperComboBoxEnabled;
            set => SetProperty(ref _wrapperComboBoxEnabled, value);
        }
        public bool BrowseButtonEnabled
        {
            get => _browseButtonEnabled;
            set => SetProperty(ref _browseButtonEnabled, value);
        }

        #endregion

        #region Commands

        public DelegateCommand PatchCommand { get; }
        public DelegateCommand UninstallCommand { get; }
        public DelegateCommand OpenFileDialogCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel(IDialogService dialogService, IOpenFileDialogService openFileDialogService)
        {
            PatchCommand = new DelegateCommand(OnClickedPatch, CanPatch);
            UninstallCommand = new DelegateCommand(OnClickedUninstall, CanUninstall);
            _openFileDialogService = openFileDialogService;
            _dialogService = dialogService;
            OpenFileDialogCommand = new DelegateCommand(OnOpenFileDialog);
            Resolutions = Sims1WidescreenPatcher.Resolutions.Resolutions.Get();
        }

        #endregion

        #region Methods

        private bool CanPatch()
        {
            return !_previouslyPatchedList.Contains(Path) && 
                   !_isBusy && 
                   !CheckForBackup() &&
                   !string.IsNullOrWhiteSpace(Path) &&
                   IsValidExe() && 
                   IsValidRes();
        }

        private bool IsValidExe()
        {
            if (new Exe().ValidFile(_path))
                return true;

            if (!_invalidExeDialogShown)
            {
                _invalidExeDialogShown = true;
                _dialogService.ShowMessageBox("Invalid Sims exe. Please replace it with a NoCD exe that has not been patched to a custom resolution.");
            }
            return false;
        }

        private bool IsValidRes()
        {
            return SelectedResolution != null &&
                   _resolutionRegex.IsMatch($"{_selectedResolution.Width}x{_selectedResolution.Height}");
        }

        private void OpenFinishedPatchPopup()
        {
            _dialogService.ShowMessageBox("Patched! You may close this application now.");
            Progress = 0;
        }

        private bool CanUninstall()
        {
            return !_isBusy && CheckForBackup();
        }

        private bool CheckForBackup()
        {
            return !string.IsNullOrWhiteSpace(Path) && FileHelper.CheckForBackup(Path);
        }

        private void EvaluateUiElements()
        {
            ResolutionComboBoxEnabled = !ResolutionComboBoxEnabled;
            WrapperComboBoxEnabled = !WrapperComboBoxEnabled;
            BrowseButtonEnabled = !BrowseButtonEnabled;
            PatchCommand.RaiseCanExecuteChanged();
            UninstallCommand.RaiseCanExecuteChanged();
        }

        private void OnClickedUninstall(object commandParameter)
        {
            _isBusy = true;
            EvaluateUiElements();
            UninstallPatch.DoUninstall(_path);
            _isBusy = false;
            _previouslyPatchedList.Remove(Path);
            _dialogService.ShowMessageBox("Uninstalled!");
            EvaluateUiElements();
        }

        private void OnClickedPatch(object commandParameter)
        {
#pragma warning disable CS4014 // Asynchronous methods should return a Task instead of void
            DoPatch();
#pragma warning restore CS4014 // Asynchronous methods should return a Task instead of void
        }

        private async Task DoPatch()
        {
            try
            {
                _isBusy = true;
                EvaluateUiElements();
                var progress = new Progress<double>(percent => { Progress = percent; });
                await Task.Run(() => new Exe().Patch(Path, SelectedResolution.Width, SelectedResolution.Height));
                await Task.Run(() => new Images().CopyGraphics(Path, SelectedResolution.Width, SelectedResolution.Height, progress));

                if (SelectedWrapper != Wrapper.None)
                {
                    await Task.Run(() => new GraphicsWrapper().TryRemoveWrapper(Path));
                    await Task.Run(() => new GraphicsWrapper().CopyDll(SelectedWrapper, Path));
                }

                _isBusy = false;
                _previouslyPatchedList.Add(Path);
                EvaluateUiElements();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error(e.Source);
                Log.Error(e.StackTrace);
            }
        }

        private void OnOpenFileDialog(object obj)
        {
            Path = _openFileDialogService.OpenFileDialog();
        }

        #endregion
    }
}