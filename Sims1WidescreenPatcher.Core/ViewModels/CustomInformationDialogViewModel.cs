namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomInformationDialogViewModel : ViewModelBase
{
    public CustomInformationDialogViewModel() : this("Default Title", "Default message.") {}
    public CustomInformationDialogViewModel(string title, string message)
    {
        Title = title;
        Message = message;
    }
    
    public string Title { get; }
    public string Message { get; }
}