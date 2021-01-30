using Serilog;
using Sims1WidescreenPatcher.Exe;
using Sims1WidescreenPatcher.IO;
using Sims1WidescreenPatcher.Media;
using Sims1WidescreenPatcher.Uninstall;
using Sims1WidescreenPatcher.Voodoo;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sims1WidescreenPatcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private string _path;
        private int _width;
        private int _height;
        private bool _dgVoodooEnabled;
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
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _path, value);
                    IsValidExe();
                    CheckForBackup();
                }
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
            set {
                SetProperty(ref _height, value);
                if (_height > 1080)
                    DgVoodooEnabled = true;
            }
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
            _openMessageBoxService = new DialogService();
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
            var isValid = await Task.Run(() => new S1WP(_path, _width, _height).ValidFile());
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
                PatchButtonEnabled = false;
            else
                PatchButtonEnabled = true;
        }

        private async void OnClickedPatch(object commandParameter)
        {
            PatchButtonEnabled = false;
            BrowseButtonEnabled = false;
            var progress = new Progress<double>(percent => { Progress = percent; });
            await Task.Run(() => new S1WP(_path, _width, _height).Patch());
            await Task.Run(() => 
            {
                var i = new Images(_path, _width, _height, progress);
                i.CopyGraphics();
            });
            if (_dgVoodooEnabled)
                await Task.Run(() => new Voodoo2(_path).ExtractVoodoo());
            BrowseButtonEnabled = true;
        }

        #endregion
    }
}
