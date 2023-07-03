namespace Sims1WidescreenPatcher.Utilities.Models;

public class NewProgressEventArgs
{
    public double Progress { get; }

    public NewProgressEventArgs(double progress) => Progress = progress;
}
