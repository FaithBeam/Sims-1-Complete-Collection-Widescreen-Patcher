namespace Sims1WidescreenPatcher.Core.ViewModels;

public interface ICheckboxViewModel
{
    string Label { get; set; }
    bool Checked { get; set; }
    string ToolTipText { get; set; }
}