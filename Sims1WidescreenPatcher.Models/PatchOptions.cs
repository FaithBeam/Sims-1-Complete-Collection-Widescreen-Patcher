using System;

namespace Sims1WidescreenPatcher.Models
{
    public class PatchOptions
    {
        public string Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool DgVoodooEnabled { get; set; }
        public IProgress<double> Progress { get; set; }
    }
}
