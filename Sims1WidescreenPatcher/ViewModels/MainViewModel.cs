using Sims1WidescreenPatcher.Exe;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Media;
using Sims1WidescreenPatcher.Uninstall;
using Sims1WidescreenPatcher.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Sims1WidescreenPatcher.Wrappers.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Sims1WidescreenPatcher.EnumerateResolutions;

namespace Sims1WidescreenPatcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private string _path;
        private bool _uninstallButtonEnabled;
        private double _progress;
        private bool _patchButtonEnabled;
        private bool _browseButtonEnabled = true;
        private readonly OpenFileDialogService _fileDialogService;
        private readonly DialogService _openMessageBoxService;
        private readonly DelegateCommand _patchCommand;
        private readonly DelegateCommand _uninstallCommand;

        #endregion

        #region Properties

        public List<Resolution> Res { get; set; }
        private Resolution _selectedResolution;
        public Resolution SelectedResolution
        {
            get => _selectedResolution;
            set => SetProperty(ref _selectedResolution, value);
        }

        public bool BrowseButtonEnabled
        {
            get => _browseButtonEnabled;
            set => _ = SetProperty(ref _browseButtonEnabled, value);
        }

        public bool UninstallButtonEnabled
        {
            get => _uninstallButtonEnabled;
            set => _ = SetProperty(ref _uninstallButtonEnabled, value);
        }

        public bool PatchButtonEnabled
        {
            get => _patchButtonEnabled;
            set => _ = SetProperty(ref _patchButtonEnabled, value);
        }

        public string Path
        {
            get => _path;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _ = SetProperty(ref _path, value);
                    IsValidExe();
                    CheckForBackup();
                }
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

        private Wrapper _selectedWrapper;
        public Wrapper SelectedWrapper
        {
            get => _selectedWrapper;
            set => SetProperty(ref _selectedWrapper, value);
        }

        public ICommand PatchCommand => _patchCommand;
        public ICommand UninstallCommand => _uninstallCommand;
        public ICommand OpenFileDialogCommand { get; set; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            _patchCommand = new DelegateCommand(OnClickedPatch);
            _uninstallCommand = new DelegateCommand(OnClickedUninstall);
            _fileDialogService = new OpenFileDialogService();
            _openMessageBoxService = new DialogService();
            OpenFileDialogCommand = new DelegateCommand(OnOpenFileDialog);
            
            Res = EnumerateResolutions.EnumerateResolutions.Get();
            _selectedResolution = Res.FirstOrDefault();
        }

        #endregion

        #region Methods

        private void OpenFinishedPatchPopup()
        {
            _openMessageBoxService.ShowMessageBox("Patched!");
            Progress = 0;
            CheckForBackup();
        }

        private void CheckForBackup()
        {
            UninstallButtonEnabled = !string.IsNullOrWhiteSpace(_path) && FileHelper.CheckForBackup(_path);
        }

        private void OnClickedUninstall(object commandParameter)
        {
            UninstallPatch.DoUninstall(_path);
            CheckForBackup();
            _openMessageBoxService.ShowMessageBox("Uninstalled!");
            IsValidExe();
        }

        private async void IsValidExe()
        {
            var isValid = await Task.Run(() => new S1WP(_path, _selectedResolution.Width, _selectedResolution.Height).ValidFile());
            if (isValid)
            {
                PatchButtonEnabled = true;
            }
            else
            {
                _openMessageBoxService.ShowMessageBox(
                    "This executable cannot be edited. Please make sure you're using a valid nocd executable.");
                PatchButtonEnabled = false;
            }
        }

        private void OnOpenFileDialog(object obj)
        {
            Path = _fileDialogService.OpenFileDialog();
            CheckPath();
        }

        private void CheckPath()
        {
            PatchButtonEnabled = !string.IsNullOrEmpty(_path);
        }

        private async void OnClickedPatch(object commandParameter)
        {
            PatchButtonEnabled = false;
            BrowseButtonEnabled = false;
            var progress = new Progress<double>(percent => { Progress = percent; });
            await Task.Run(() => new S1WP(_path, _selectedResolution.Width, _selectedResolution.Height).Patch());
            await Task.Run(() =>
            {
                var i = new Images(_path, _selectedResolution.Width, _selectedResolution.Height, progress);
                i.CopyGraphics();
            });

            if (_selectedWrapper != Wrapper.None)
            {
                await Task.Run(() => new DllWrapper(_path).CopyDll(_selectedWrapper));
            }

            BrowseButtonEnabled = true;
        }

        #endregion
    }
}