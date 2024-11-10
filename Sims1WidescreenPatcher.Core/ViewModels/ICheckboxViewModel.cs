namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICheckboxViewModel
{
    string Label { get; set; }
    string ToolTipText { get; set; }
    bool Checked { get; set; }
    bool IsEnabled { get; }
}
