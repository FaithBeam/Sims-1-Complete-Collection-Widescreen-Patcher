using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// https://stackoverflow.com/a/744609
namespace Sims1WidescreenPatcher.Resolutions
{
    public class EnumerateResolutions
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        public EnumerateResolutions()
        {
        }

        public void Get(ObservableCollection<Resolution> resolutions)
        {
            var vDevMode = new DEVMODE();
            int i = 0;
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                var resolution = new Resolution
                {
                    Width = vDevMode.dmPelsWidth,
                    Height = vDevMode.dmPelsHeight
                };
                if (!resolutions.Any(r => r.Width == resolution.Width && r.Height == r.Height))
                {
                    resolutions.Add(resolution);
                }
                i++;
            }
        }
    }

    public class Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
