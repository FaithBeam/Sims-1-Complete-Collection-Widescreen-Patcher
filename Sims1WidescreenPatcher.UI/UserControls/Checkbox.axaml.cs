using Avalonia.ReactiveUI;
using Sims1WidescreenPatcher.Core.ViewModels;

namespace Sims1WidescreenPatcher.UI.UserControls;

public partial class Checkbox : ReactiveUserControl<CheckboxViewModel>
{
    public Checkbox()
    {
        InitializeComponent();
    }
}