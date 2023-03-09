﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChatQRCapture
{
    internal class Program
    {
        /// <summary>
        /// 目标平台：x64，因为带的那两个C语言的dll（OpenCV + WeChatQRCode）是x64的。
        /// 
        /// 先打开PC版微信,打开有二维码的微信小程序，让其显示二维码
        /// 
        /// 一个微信小程序启动之后，会有很多相似的微信小程序进程，这时候需要根据微信
        /// 小程序的MainWindowTitle来判断应该选择哪一个进程。这里可以通过向GetWeChatAppQrCodeInfo()
        /// 方法传入标题参数的方式进行过滤。默认参数为“校园卡”，比如：GetWeChatAppQrCodeInfo("校园卡")。
        /// 
        /// 找到第一个符合条件的进程，就将该窗口调到最前面，然后进行全屏截图，接下来裁剪窗口、存盘为一个
        /// 临时文件（调用别人写的dll，只能从磁盘加载图片文件，进行解码）。
        /// [直接调用WinApi对微信小程序窗口截图，得到的是黑屏，因此使用全屏截图+裁剪的方式]
        /// 
        /// 执行该程序，就会输出微信小程序中二维码的文本信息
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var qrCodeText = QrcodeCapture.GetWeChatAppQrCodeInfo();
            Console.WriteLine("二维码字符串内容：" + qrCodeText);
            Console.ReadKey();
        }
    }
}