using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Utilities.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface IMainWindowViewModel
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
    bool SortByAspectRatio { get; set; }
    bool IsResolutionsColored { get; set; }
    ReadOnlyObservableCollection<AspectRatio> AspectRatios { get; }
    ReadOnlyObservableCollection<Resolution> FilteredResolutions { get; }
    Resolution? SelectedResolution { get; set; }
    AvaloniaList<IWrapper> Wrappers { get; }
    int SelectedWrapperIndex { get; set; }
    double Progress { get; }
    IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing { get; }
    IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed { get; }
    IObservable<Exception> ThrownExceptions { get; }
    IDisposable SuppressChangeNotifications();
    bool AreChangeNotificationsEnabled();
    IDisposable DelayChangeNotifications();
    event PropertyChangingEventHandler? PropertyChanging;
    event PropertyChangedEventHandler? PropertyChanged;
}