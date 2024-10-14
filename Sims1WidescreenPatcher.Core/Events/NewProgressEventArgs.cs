namespace Sims1WidescreenPatcher.Core.Events;

public class NewProgressEventArgs
{
    public double Progress { get; }
    public string Status { get; }
    public string Status2 { get; }

    public NewProgressEventArgs(double progress) : this(progress, string.Empty, string.Empty)
    {
    }

    public NewProgressEventArgs(double progress, string status, string status2)
    {
        Progress = progress;
        Status = status;
        Status2 = status2;
    }
}
