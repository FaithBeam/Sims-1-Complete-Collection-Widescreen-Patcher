using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using Sims1WidescreenPatcher.EnumerateResolutions;

namespace Sims1WidescreenPatcher.Resolutions.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGet()
        {
            var c = new EnumerateResolutions.EnumerateResolutions();
            var t = EnumerateResolutions.EnumerateResolutions.Get();
            ;
        }
    }
}
