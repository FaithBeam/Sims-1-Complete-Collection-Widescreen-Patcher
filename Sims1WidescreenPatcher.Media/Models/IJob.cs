namespace Sims1WidescreenPatcher.Media.Models
{
    public interface IJob
    {
        byte[] Bytes { get; set; }
        string Output { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        void DoWork();
    }
}