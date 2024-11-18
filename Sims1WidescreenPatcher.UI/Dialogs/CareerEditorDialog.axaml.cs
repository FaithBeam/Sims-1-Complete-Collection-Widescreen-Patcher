using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.Dialogs;

public partial class CareerEditorDialog : ReactiveWindow<CareerEditorDialogViewModel>
{
    public CareerEditorDialog()
    {
        InitializeComponent();
    }
}
