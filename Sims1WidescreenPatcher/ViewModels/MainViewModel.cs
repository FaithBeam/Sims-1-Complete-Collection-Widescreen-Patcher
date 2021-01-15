using Sims1WidescreenPatcher.Exe;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Models;
using Sims1WidescreenPatcher.Uninstall;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sims1WidescreenPatcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        string _path;
        int _width;
        int _height;
        bool _dgVoodooEnabled;
        bool _uninstallButtonEnabled;
        double _progress;
        bool _patchButtonEnabled;
        bool _browseButtonEnabled = true;
        readonly OpenFileDialogService _fileDialogService;
        readonly MessageBoxService _openMessageBoxService;
        readonly DelegateCommand _patchCommand;
        readonly DelegateCommand _uninstallCommand;

        #endregion

        #region Properties

        public bool BrowseButtonEnabled
        {
            get { return _browseButtonEnabled; }
            set { SetProperty(ref _browseButtonEnabled, value); }
        }

        public bool UninstallButtonEnabled
        {
            get { return _uninstallButtonEnabled; }
            set { SetProperty(ref _uninstallButtonEnabled, value); }
        }

        public bool PatchButtonEnabled
        {
            get { return _patchButtonEnabled; }
            set { SetProperty(ref _patchButtonEnabled, value); }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                SetProperty(ref _path, value);
                IsValidExe();
                CheckForBackup();
            }
        }

        public int Width
        {
            get { return _width; }
            set
            {
                SetProperty(ref _width, value);
                if (_width > 1920)
                    DgVoodooEnabled = true;
            }
        }

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public bool DgVoodooEnabled
        {
            get { return _dgVoodooEnabled; }
            set { SetProperty(ref _dgVoodooEnabled, value); }
        }

        public double Progress
        {
            get { return _progress; }
            set
            {
                SetProperty(ref _progress, value);
                if (_progress >= 100)
                {
                    OpenFinishedPatchPopup();
                }
            }
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
            _openMessageBoxService = new MessageBoxService();
            OpenFileDialogCommand = new DelegateCommand(OnOpenFileDialog);
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
            if (!string.IsNullOrWhiteSpace(_path) && FileHelper.CheckForBackup(_path))
                UninstallButtonEnabled = true;
            else
                UninstallButtonEnabled = false;
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
            var isValid = await Task.Run(() => new S1WP(new PatchOptions { Path = _path }).ValidFile());
            if (isValid)
            {
                PatchButtonEnabled = true;
            }
            else
            {
                _openMessageBoxService.ShowMessageBox("This executable cannot be edited. Please make sure you're using a valid nocd executable.");
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
            if (string.IsNullOrEmpty(_path))
            {
                PatchButtonEnabled = false;
            }
            else
            {
                PatchButtonEnabled = true;
            }
        }

        private async void OnClickedPatch(object commandParameter)
        {
            PatchButtonEnabled = false;
            BrowseButtonEnabled = false;
            var progress = new Progress<double>(percent => { Progress = percent; });
            await Task.Run(() => new S1WP(new PatchOptions
            {
                Progress = progress,
                DgVoodooEnabled = _dgVoodooEnabled,
                Height = _height,
                Path = _path,
                Width = _width
            }).Patch());
            BrowseButtonEnabled = true;
        }

        #endregion
    }
}
