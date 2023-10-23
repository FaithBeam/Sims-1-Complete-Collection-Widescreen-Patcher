using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using ReactiveUI;

namespace Sims1WidescreenPatcher.Core.ViewModels;

public class NotificationViewModel : ViewModelBase, INotificationViewModel
{
    private bool _isVisible;
    private bool _hasBeenShown;
    
    public NotificationViewModel()
    {
        WikiCommand = ReactiveCommand.Create(() =>
            OpenUrl("https://github.com/FaithBeam/Sims-1-Complete-Collection-Widescreen-Patcher/wiki"));
    }

    public ReactiveCommand<Unit, Unit> WikiCommand { get; }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public bool HasBeenShown
    {
        get => _hasBeenShown;
        set => this.RaiseAndSetIfChanged(ref _hasBeenShown, value);
    }
    
    // https://stackoverflow.com/a/43232486
    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}