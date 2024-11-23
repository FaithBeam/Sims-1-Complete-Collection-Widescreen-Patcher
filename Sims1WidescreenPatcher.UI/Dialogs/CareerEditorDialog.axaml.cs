using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Dialogs;

public partial class CareerEditorDialog : ReactiveWindow<CareerEditorDialogViewModel>
{
    private TopLevel? _topLevel;
    private Window? _window;

    public CareerEditorDialog()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (ViewModel == null)
            {
                return;
            }
            _topLevel = GetTopLevel(this);
            _window = (Window)_topLevel!;

            d(ViewModel.ShowOpenFileDialogInteraction.RegisterHandler(ShowOpenFileDialogAsync));
        });
    }

    private void ExitMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CancelBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async Task ShowOpenFileDialogAsync(IInteractionContext<Unit, IStorageFile?> interaction)
    {
        if (_topLevel != null)
        {
            var fileNames = await _topLevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Select work.iff",
                    AllowMultiple = false,
                    FileTypeFilter = new FilePickerFileType[]
                    {
                        new("work.iff") { Patterns = new[] { "work.iff" } },
                    },
                }
            );
            interaction.SetOutput(fileNames.Any() ? fileNames[0] : null);
        }
    }
}
