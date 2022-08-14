using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Views;

public class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d =>
        {
            if (ViewModel != null) d(ViewModel.ShowOpenFileDialog.RegisterHandler(ShowOpenFileDialog));
            d(ViewModel!.ShowCustomResolutionDialog.RegisterHandler(ShowCustomResolutionDialogAsync));
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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
                    Extensions = new List<string> {"exe"},
                    Name = "Sims"
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