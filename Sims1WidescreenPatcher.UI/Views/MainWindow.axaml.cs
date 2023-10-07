using System.Collections.Generic;
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
using Splat;

namespace Sims1WidescreenPatcher.UI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = Locator.Current.GetService<MainWindowViewModel>();
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

    private async Task ShowCustomResolutionDialogAsync(
        InteractionContext<CustomResolutionDialogViewModel, Resolution?> interaction)
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
}