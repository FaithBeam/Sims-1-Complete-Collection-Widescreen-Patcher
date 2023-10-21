using Avalonia.ReactiveUI;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Tabs;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class OptionalTab : ReactiveUserControl<IOptionalTabViewModel>
{
    public OptionalTab()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            Width = UiDimensions.Width;
            Height = UiDimensions.Height;
        });
    }
}