using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace Sims1WidescreenPatcher.Resolutions.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGet()
        {
            var o = new ObservableCollection<Resolution>();
            var c = new EnumerateResolutions();
            c.Get(o);
            ;
        }
    }
}
