using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BioImage
{
    public class Win32
    {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
    }
}
