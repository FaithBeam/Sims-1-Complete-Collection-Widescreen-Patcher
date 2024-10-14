namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomInformationDialogViewModel(string title, string message) : ViewModelBase
{
    public string Title { get; } = title;
    public string Message { get; } = message;
}