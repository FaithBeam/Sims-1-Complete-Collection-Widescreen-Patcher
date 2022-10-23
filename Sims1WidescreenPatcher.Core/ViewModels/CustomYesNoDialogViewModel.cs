using System.Reactive;
using ReactiveUI;
using Sims1WidescreenPatcher.Core.Models;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class CustomYesNoDialogViewModel : ViewModelBase
{
    public CustomYesNoDialogViewModel() : this("Default Title", "Default message.")
    {
    }

    public CustomYesNoDialogViewModel(string title, string message)
    {
        Title = title;
        Message = message;
        YesCommand = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = true });
        NoCommand = ReactiveCommand.Create(() => new YesNoDialogResponse { Result = false });
    }

    public ReactiveCommand<Unit, YesNoDialogResponse> YesCommand { get; }
    public ReactiveCommand<Unit, YesNoDialogResponse> NoCommand { get; }

    public string Title { get; }
    public string Message { get; }
}