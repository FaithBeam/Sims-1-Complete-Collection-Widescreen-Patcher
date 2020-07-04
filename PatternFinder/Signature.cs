using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternFinder
{
    public class Signature
    {
        public Signature(string name, Pattern.Byte[] pattern)
        {
            Name = name;
            Pattern = pattern;
            FoundOffset = -1;
        }

        public Signature(string name, string pattern)
        {
            Name = name;
            Pattern = PatternFinder.Pattern.Transform(pattern);
            FoundOffset = -1;
        }

        public string Name { get; private set; }
        public Pattern.Byte[] Pattern { get; private set; }
        public long FoundOffset;

        public override string ToString()
        {
            return Name;
        }
    }
}
