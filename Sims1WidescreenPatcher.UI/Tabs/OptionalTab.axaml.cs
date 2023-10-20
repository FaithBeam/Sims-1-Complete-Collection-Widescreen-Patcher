using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sims1WidescreenPatcher.Core.Tabs;
using Splat;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class OptionalTab : UserControl
{
    public OptionalTab() : this(Locator.Current.GetService<IOptionalTabViewModel>() ?? throw new InvalidOperationException())
    {
    }

    public OptionalTab(IOptionalTabViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}