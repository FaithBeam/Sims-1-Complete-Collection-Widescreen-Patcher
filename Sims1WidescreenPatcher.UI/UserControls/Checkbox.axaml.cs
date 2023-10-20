using Avalonia.Input;
using Avalonia.ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.UserControls;

public partial class Checkbox : ReactiveUserControl<CheckboxViewModel>
{
    public Checkbox()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        ViewModel.Checked = !ViewModel.Checked;
    }
}