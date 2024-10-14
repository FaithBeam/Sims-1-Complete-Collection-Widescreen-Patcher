namespace Sims1WidescreenPatcher.Core.Events;

public class NewProgressEventArgs(double progress, string status, string status2)
{
    public double Progress { get; } = progress;
    public string Status { get; } = status;
    public string Status2 { get; } = status2;

    public NewProgressEventArgs(double progress) : this(progress, string.Empty, string.Empty)
    {
    }
}
