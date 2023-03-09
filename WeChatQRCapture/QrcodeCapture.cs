using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace WeChatQRCapture
{
    public static class QrcodeCapture
    {
        //微信小程序进程
        static Process wxAppProcess = null;

        /// <summary>
        /// 遍历系统进程，获取第一个窗口标题含有特定字符串的微信小程序
        /// </summary>
        /// <param name="mainWindowTitle"></param>
        /// <returns></returns>
        private static Boolean CheckWeChatApp(string mainWindowTitle)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                if (process.ProcessName == "WeChatAppEx" && process.MainWindowTitle.Contains(mainWindowTitle))
                {
                    wxAppProcess = process;
                    return true;
                }
            }
            return false;
        }

        public static string GetWeChatAppQrCodeInfo(string mainWindowTitle = "校园卡")
        {
            //检查微信小程序是否启动
            CheckWeChatApp(mainWindowTitle);
            if (wxAppProcess == null)
            {
                MessageBox.Show("窗口标题含有\"" + mainWindowTitle + "\"的微信小程序未找到！");
                return null;
            }

            //1）获取设备上下文句柄
            var windowHandle = wxAppProcess.MainWindowHandle;
            if (windowHandle == IntPtr.Zero) return null;

            IntPtr windowDCHandle = Win32Helper.GetWindowDC(IntPtr.Zero);
            if (windowDCHandle == IntPtr.Zero)
            {
                Win32Helper.ReleaseDC(windowHandle, windowDCHandle);
                MessageBox.Show("获取设备上下文句柄失败！");
                return null;
            }

            //2）获取微信小程序窗口边界和尺寸：GetWindowRect，
            Rectangle rectangle = new Rectangle();
            if (Win32Helper.GetWindowRect(windowHandle, ref rectangle) == 0)
            {
                Win32Helper.ReleaseDC(windowHandle, windowDCHandle);
                MessageBox.Show("获取指定窗口边界和尺寸失败！");
                return null;
            };

            //3）计算窗口大小
            //注意C#中的Rectangle与C++中RECT区别
            int width = rectangle.Width - rectangle.X;
            int height = rectangle.Height - rectangle.Y;

            //4）对微信小程序进行截图
            //直接调用WinApi对微信小程序窗口截图，得到的是黑屏，因此使用全屏截图+裁剪的方式
            Win32Helper.SetForegroundWindow(windowHandle);
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(rectangle.X, rectangle.Y, 0, 0, rectangle.Size);
            graphics.DrawImage(bitmap, 0, 0, rectangle, GraphicsUnit.Pixel);
            graphics.Dispose();

            //5）清理垃圾
            Win32Helper.ReleaseDC(windowHandle, windowDCHandle);

            //这里用了别人的dll，无法从内存中直接使用bitmap，因此按照要求先存盘，然后再读取。
            //https://download.csdn.net/download/vokxchh/85278860
            var location = Path.GetTempFileName();
            bitmap.Save(location);

            //解码
            try
            {
                IntPtr imgPath = Marshal.StringToHGlobalAnsi(location);
                Win32Helper.Result[] results = new Win32Helper.Result[20];

                if (Win32Helper.Decoder(imgPath, results) == true)
                {
                    return results[0].result;
                }
            }
            catch { }

            Process.Start("mspaint.exe", location);
            MessageBox.Show("解码失败！请检查“模型”文件和“dll”文件！");
            return null;
        }
    }
}
