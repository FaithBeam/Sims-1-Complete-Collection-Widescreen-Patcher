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
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class MainWindow : ReactiveWindow<IMainWindowViewModel>
{
    public MainWindow(IMainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        this.WhenActivated(d =>
        {
            if (ViewModel == null) return;
            d(ViewModel.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialogAsync));
            d(ViewModel!.ShowCustomResolutionDialog.RegisterHandler(ShowCustomResolutionDialogAsync));
            d(ViewModel!.ShowCustomYesNoDialog.RegisterHandler(ShowCustomYesNoDialogAsync));
            d(ViewModel!.ShowCustomInformationDialog.RegisterHandler(ShowCustomInformationDialogAsync));
        });
    }

    private async Task ShowCustomInformationDialogAsync(
        InteractionContext<CustomInformationDialogViewModel, Unit> interaction)
    {
        var dialog = new CustomInformationDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Unit>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowCustomYesNoDialogAsync(
        InteractionContext<CustomYesNoDialogViewModel, YesNoDialogResponse?> interaction)
    {
        var dialog = new CustomYesNoDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<YesNoDialogResponse?>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowCustomResolutionDialogAsync(InteractionContext<ICustomResolutionDialogViewModel, Resolution?> interaction)
    {
        var dialog = new CustomResolutionDialog
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<Resolution?>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowOpenFileDialogAsync(InteractionContext<Unit, IStorageFile?> interaction)
    {
        var fileNames = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Sims.exe",
            AllowMultiple = false,
            FileTypeFilter = new FilePickerFileType[] {new ("Sims.exe") {Patterns = new []{"Sims.exe"}}}
        });
        interaction.SetOutput(fileNames.Any() ? fileNames[0] : null);
    }

    private void IsResolutionsColoredLabel_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }
        ViewModel.IsResolutionsColored = !ViewModel.IsResolutionsColored;
    }

    private void SortByAspectRatioLabel_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }
        ViewModel.SortByAspectRatio = !ViewModel.SortByAspectRatio;
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