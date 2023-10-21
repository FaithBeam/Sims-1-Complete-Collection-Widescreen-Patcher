using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.UI.Views;
using CustomInformationDialog = Sims1WidescreenPatcher.UI.Dialogs.CustomInformationDialog;
using CustomResolutionDialog = Sims1WidescreenPatcher.UI.Dialogs.CustomResolutionDialog;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class MainTab : ReactiveUserControl<IMainTabViewModel>
{
    private TopLevel? _topLevel;
    private Window? _window;
    
    public MainTab()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            if (ViewModel == null) return;
            _topLevel = TopLevel.GetTopLevel(this);
            _window = (Window)_topLevel!;
            d(ViewModel.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialogAsync));
            d(ViewModel.ShowCustomResolutionDialog.RegisterHandler(ShowCustomResolutionDialogAsync));
            d(ViewModel.ShowCustomYesNoDialog.RegisterHandler(ShowCustomYesNoDialogAsync));
            d(ViewModel.ShowCustomInformationDialog.RegisterHandler(ShowCustomInformationDialogAsync));
            UiDimensions.Width = _window.Width;
            UiDimensions.Height = _window.Height;
        });
    }

    private async Task ShowCustomInformationDialogAsync(IInteractionContext<CustomInformationDialogViewModel, Unit> interaction)
    {
        var dialog = new CustomInformationDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Unit>(_window ?? throw new InvalidOperationException());
        interaction.SetOutput(result);
    }

    private async Task ShowCustomYesNoDialogAsync(IInteractionContext<CustomYesNoDialogViewModel, YesNoDialogResponse?> interaction)
    {
        var dialog = new CustomYesNoDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<YesNoDialogResponse?>(_window ?? throw new InvalidOperationException());
        interaction.SetOutput(result);
    }

    private async Task ShowCustomResolutionDialogAsync(IInteractionContext<ICustomResolutionDialogViewModel, Resolution?> interaction)
    {
        var dialog = new CustomResolutionDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Resolution?>(_window ?? throw new InvalidOperationException());
        interaction.SetOutput(result);
    }

    private async Task ShowOpenFileDialogAsync(IInteractionContext<Unit, IStorageFile?> interaction)
    {
        if (_topLevel != null)
        {
            var fileNames = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Sims.exe",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] {new ("Sims.exe") {Patterns = new []{"Sims.exe"}}}
            });
            interaction.SetOutput(fileNames.Any() ? fileNames[0] : null);
        }
    }

    private void AspectRatioComboBox_OnTapped(object sender, TappedEventArgs e)
    {
        switch (e.KeyModifiers)
        {
            case KeyModifiers.Control:
                if (ViewModel is null)
                {
                    return;
                }

                ViewModel.SelectedAspectRatio = null;
                var combo = (ComboBox)sender;
                combo.IsDropDownOpen = false;
                break;
            case KeyModifiers.None:
            case KeyModifiers.Alt:
            case KeyModifiers.Shift:
            case KeyModifiers.Meta:
            default:
                return;
        }
    }
}