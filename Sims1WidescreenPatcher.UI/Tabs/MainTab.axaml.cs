using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using Avalonia.Styling;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;
using Sims1WidescreenPatcher.Core.Tabs;
using Sims1WidescreenPatcher.Core.ViewModels;
using Sims1WidescreenPatcher.UI.Converters;
using CustomInformationDialog = Sims1WidescreenPatcher.UI.Dialogs.CustomInformationDialog;
using CustomResolutionDialog = Sims1WidescreenPatcher.UI.Dialogs.CustomResolutionDialog;
using CustomYesNoDialog = Sims1WidescreenPatcher.UI.Dialogs.CustomYesNoDialog;

namespace Sims1WidescreenPatcher.UI.Tabs;

public partial class MainTab : ReactiveUserControl<MainTabViewModel>
{
    private TopLevel? _topLevel;
    private Window? _window;

    private readonly Style _cbShouldColorBackgroundStyle = new(x =>
        x.OfType<ComboBox>().Class("ShouldColor").Child().OfType<ComboBoxItem>()
    )
    {
        Setters =
        {
            new Setter(
                BackgroundProperty,
                new Binding
                {
                    Converter = new ResolutionColorCodingConverter(),
                    ConverterParameter = "Background",
                }
            ),
        },
    };
    private readonly Style _cbShouldColorPointerOverStyle = new(x =>
        x.OfType<ComboBox>()
            .Class("ShouldColor")
            .Child()
            .OfType<ComboBoxItem>()
            .Class(":pointerover")
            .Template()
            .OfType<ContentPresenter>()
    )
    {
        Setters =
        {
            new Setter(
                BackgroundProperty,
                new Binding
                {
                    Converter = new ResolutionColorCodingConverter(),
                    ConverterParameter = "Pointerover",
                }
            ),
        },
    };
    private readonly Style _cbShouldColorSelectedStyle = new(x =>
        x.OfType<ComboBox>()
            .Class("ShouldColor")
            .Child()
            .OfType<ComboBoxItem>()
            .Class(":selected")
            .Template()
            .OfType<ContentPresenter>()
    )
    {
        Setters =
        {
            new Setter(
                BackgroundProperty,
                new Binding
                {
                    Converter = new ResolutionColorCodingConverter(),
                    ConverterParameter = "Selected",
                }
            ),
        },
    };

    public MainTab()
    {
        InitializeComponent();

        ResolutionCombo.Styles.AddRange(
            new[]
            {
                _cbShouldColorBackgroundStyle,
                _cbShouldColorPointerOverStyle,
                _cbShouldColorSelectedStyle,
            }
        );

        this.WhenActivated(d =>
        {
            _topLevel = TopLevel.GetTopLevel(this);
            _window = (Window)_topLevel!;

            this.WhenAnyValue(x => x.ViewModel)
                .WhereNotNull()
                .Subscribe(viewModel =>
                {
                    this.Bind(viewModel, vm => vm.Path, v => v.FileDialog.Text).DisposeWith(d);

                    this.BindCommand(viewModel, vm => vm.OpenFile, v => v.BrowseButton)
                        .DisposeWith(d);

                    this.OneWayBind(
                            viewModel,
                            vm => vm.AspectRatios,
                            v => v.AspectRatioComboBox.ItemsSource
                        )
                        .DisposeWith(d);
                    this.Bind(
                            viewModel,
                            vm => vm.SelectedAspectRatio,
                            v => v.AspectRatioComboBox.SelectedItem
                        )
                        .DisposeWith(d);

                    this.Bind(
                            viewModel,
                            vm => vm.ResolutionsColoredCbVm,
                            v => v.ResolutionsColoredCheckbox.DataContext
                        )
                        .DisposeWith(d);

                    this.Bind(
                            viewModel,
                            vm => vm.SortByAspectRatioCbVm,
                            v => v.SortByAspectRatioCheckbox.DataContext
                        )
                        .DisposeWith(d);

                    AspectRatioComboBox.ItemTemplate = new FuncDataTemplate<AspectRatio?>(
                        (value, _) =>
                            new TextBlock
                            {
                                [TextBlock.TextProperty] = value?.ToString() ?? string.Empty,
                            }
                    );
                    ResolutionCombo.ItemTemplate = new FuncDataTemplate<Resolution?>(
                        (r, _) =>
                            new TextBlock
                            {
                                [TextBlock.TextProperty] = r?.ToString() ?? string.Empty,
                            }
                    );
                    WrapperCombo.ItemTemplate = new FuncDataTemplate<IWrapper?>(
                        (value, _) =>
                            new TextBlock { [TextBlock.TextProperty] = value?.Name ?? string.Empty }
                    );
                    this.OneWayBind(
                            viewModel,
                            vm => vm.FilteredResolutions,
                            v => v.ResolutionCombo.ItemsSource
                        )
                        .DisposeWith(d);
                    viewModel
                        .WhenAnyValue(x => x.ResolutionsColoredCbVm!.Checked)
                        .Subscribe(@checked =>
                        {
                            if (@checked)
                            {
                                ResolutionCombo.Classes.Add("ShouldColor");
                            }
                            else
                            {
                                if (ResolutionCombo.Classes.Contains("ShouldColor"))
                                {
                                    ResolutionCombo.Classes.Remove("ShouldColor");
                                }
                            }
                        })
                        .DisposeWith(d);
                    this.Bind(
                            viewModel,
                            vm => vm.SelectedResolution,
                            v => v.ResolutionCombo.SelectedItem
                        )
                        .DisposeWith(d);

                    this.BindCommand(
                            viewModel,
                            vm => vm.CustomResolutionCommand,
                            v => v.AddResolutionBtn
                        )
                        .DisposeWith(d);

                    this.OneWayBind(viewModel, vm => vm.Wrappers, v => v.WrapperCombo.ItemsSource)
                        .DisposeWith(d);
                    this.Bind(
                            viewModel,
                            vm => vm.SelectedWrapperIndex,
                            v => v.WrapperCombo.SelectedIndex
                        )
                        .DisposeWith(d);

                    this.BindCommand(viewModel, vm => vm.UninstallCommand, v => v.UninstallButton)
                        .DisposeWith(d);

                    this.BindCommand(viewModel, vm => vm.PatchCommand, v => v.PatchButton)
                        .DisposeWith(d);
                    this.Bind(
                            viewModel,
                            vm => vm.PatchButtonToolTipTxt,
                            v => v.PatchBtnToolTipTxt.Text
                        )
                        .DisposeWith(d);

                    this.OneWayBind(
                            viewModel,
                            vm => vm.ProgressStatus,
                            v => v.ProgressStatusLabel.Content
                        )
                        .DisposeWith(d);
                    this.OneWayBind(
                            viewModel,
                            vm => vm.ProgressStatus2,
                            v => v.ProgressStatus2Label.Content
                        )
                        .DisposeWith(d);
                    this.OneWayBind(viewModel, vm => vm.Progress, v => v.ProgressBar.Value)
                        .DisposeWith(d);

                    if (viewModel.ShowOpenFileDialog != null)
                    {
                        this.BindInteraction(
                            viewModel,
                            vm => vm.ShowOpenFileDialog!,
                            ShowOpenFileDialogAsync
                        );
                    }
                    if (viewModel.ShowCustomResolutionDialog != null)
                    {
                        this.BindInteraction(
                                viewModel,
                                vm => vm.ShowCustomResolutionDialog!,
                                ShowCustomResolutionDialogAsync
                            )
                            .DisposeWith(d);
                    }
                    if (viewModel.ShowCustomYesNoDialog != null)
                    {
                        this.BindInteraction(
                                viewModel,
                                vm => vm.ShowCustomYesNoDialog!,
                                ShowCustomYesNoDialogAsync
                            )
                            .DisposeWith(d);
                    }
                    if (viewModel.ShowCustomInformationDialog != null)
                    {
                        this.BindInteraction(
                                viewModel,
                                vm => vm.ShowCustomInformationDialog!,
                                ShowCustomInformationDialogAsync
                            )
                            .DisposeWith(d);
                    }
                })
                .DisposeWith(d);
        });
    }

    private async Task ShowCustomInformationDialogAsync(
        IInteractionContext<CustomInformationDialogViewModel, Unit> interaction
    )
    {
        var dialog = new CustomInformationDialog { DataContext = interaction.Input };

        var result = await dialog.ShowDialog<Unit>(
            _window ?? throw new InvalidOperationException()
        );
        interaction.SetOutput(result);
    }

    private async Task ShowCustomYesNoDialogAsync(
        IInteractionContext<CustomYesNoDialogViewModel?, YesNoDialogResponse?> interaction
    )
    {
        var dialog = new CustomYesNoDialog { DataContext = interaction.Input };

        var result = await dialog.ShowDialog<YesNoDialogResponse?>(
            _window ?? throw new InvalidOperationException()
        );
        interaction.SetOutput(result);
    }

    private async Task ShowCustomResolutionDialogAsync(
        IInteractionContext<ICustomResolutionDialogViewModel?, Resolution?> interaction
    )
    {
        var dialog = new CustomResolutionDialog { DataContext = interaction.Input };

        var result = await dialog.ShowDialog<Resolution?>(
            _window ?? throw new InvalidOperationException()
        );
        interaction.SetOutput(result);
    }

    private async Task ShowOpenFileDialogAsync(IInteractionContext<Unit, IStorageFile?> interaction)
    {
        if (_topLevel != null)
        {
            var fileNames = await _topLevel.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Select Sims.exe",
                    AllowMultiple = false,
                    FileTypeFilter = new FilePickerFileType[]
                    {
                        new("Sims.exe") { Patterns = new[] { "Sims.exe" } },
                    },
                }
            );
            interaction.SetOutput(fileNames.Any() ? fileNames[0] : null);
        }
    }

    private void AspectRatioComboBox_OnTapped(object sender, TappedEventArgs e)
    {
        switch (e.KeyModifiers)
        {
            case KeyModifiers.Control:
                if (ViewModel is null)
                {
                    return;
                }

                ViewModel.SelectedAspectRatio = null;
                var combo = (ComboBox)sender;
                combo.IsDropDownOpen = false;
                break;
            case KeyModifiers.None:
            case KeyModifiers.Alt:
            case KeyModifiers.Shift:
            case KeyModifiers.Meta:
            default:
                return;
        }
    }
}
