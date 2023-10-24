using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class ExtrasTab : ReactiveUserControl<IExtrasTabViewModel>
{
    public ExtrasTab()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            // Width = UiDimensions.Width;
            // Height = UiDimensions.Height;
        });
    }
}