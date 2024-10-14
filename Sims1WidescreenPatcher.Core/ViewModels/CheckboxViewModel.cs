using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CheckboxViewModel(string label, string toolTipText) : ReactiveObject, ICheckboxViewModel
{
    private bool _checked;
    private bool _isEnabled = true;

    public string Label { get; set; } = label;

    public string ToolTipText { get; set; } = toolTipText;

    public bool Checked
    {
        get => _checked;
        set => this.RaiseAndSetIfChanged(ref _checked, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }
}