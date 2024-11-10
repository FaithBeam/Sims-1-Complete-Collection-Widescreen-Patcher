using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.Core.Tabs;

public interface IMainTabViewModel
{
    ICommand PatchCommand { get; }
    ICommand UninstallCommand { get; }
    ICommand OpenFile { get; }
    Interaction<Unit, IStorageFile?> ShowOpenFileDialog { get; }
    ICommand CustomResolutionCommand { get; }
    Interaction<ICustomResolutionDialogViewModel, Resolution?> ShowCustomResolutionDialog { get; }
    Interaction<CustomYesNoDialogViewModel, YesNoDialogResponse?> ShowCustomYesNoDialog { get; }
    Interaction<CustomInformationDialogViewModel, Unit> ShowCustomInformationDialog { get; }
    string Path { get; set; }
    AspectRatio? SelectedAspectRatio { get; set; }
    ReadOnlyObservableCollection<AspectRatio> AspectRatios { get; }
    ReadOnlyObservableCollection<Resolution> FilteredResolutions { get; }
    Resolution? SelectedResolution { get; set; }
    AvaloniaList<IWrapper> Wrappers { get; }
    int SelectedWrapperIndex { get; set; }
    double Progress { get; }
    public ICheckboxViewModel ResolutionsColoredCbVm { get; }
    public ICheckboxViewModel SortByAspectRatioCbVm { get; }
}
