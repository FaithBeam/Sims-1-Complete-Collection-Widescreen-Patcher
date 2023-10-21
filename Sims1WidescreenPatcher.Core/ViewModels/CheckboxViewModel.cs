using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CheckboxViewModel : ReactiveObject, ICheckboxViewModel
{
    private bool _checked;

    public CheckboxViewModel(string label, string toolTipText)
    {
        Label = label;
        ToolTipText = toolTipText;
    }

    public string Label { get; set; }
    
    public string ToolTipText { get; set; }
    
    public bool Checked
    {
        get => _checked;
        set => this.RaiseAndSetIfChanged(ref _checked, value);
    }
}