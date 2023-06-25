using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
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
            d(ViewModel.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialog));
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

    private async Task ShowOpenFileDialog(InteractionContext<Unit, string> interaction)
    {
        var dialog = new OpenFileDialog
        {
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new List<string> { "exe" },
                    Name = "Sims executable (Sims.exe)"
                }
            },
            AllowMultiple = false,
            Title = "Select Sims.exe"
        };
        var fileNames = await dialog.ShowAsync(this);
        if (fileNames is not null && fileNames.Any())
        {
            interaction.SetOutput(fileNames[0]);
        }
        else
        {
            interaction.SetOutput("");
        }
    }
}