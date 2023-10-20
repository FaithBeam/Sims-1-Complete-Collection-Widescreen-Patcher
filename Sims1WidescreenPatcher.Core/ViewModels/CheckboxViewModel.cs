using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CheckboxViewModel : ReactiveObject, ICheckboxViewModel
{
    private bool _checked;

    public CheckboxViewModel(string label)
    {
        Label = label;
    }

    public CheckboxViewModel() : this("Default label") {}

    public string Label { get; }
    
    public bool Checked
    {
        get => _checked;
        set => this.RaiseAndSetIfChanged(ref _checked, value);
    }
}