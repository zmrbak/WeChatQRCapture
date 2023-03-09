using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WeChatQRCapture
{
    public class Win32Helper
    {
        [DllImport("User32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #region used in QrCode Decode
        [DllImport("opencv_world455_cs.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static bool Decoder(IntPtr path, [Out] Result[] re);
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1), Serializable]
        public struct Result
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]//4
            public string result;
            public float x1;
            public float y1;
            public float x2;
            public float y2;
        }
        #endregion
    }
}
