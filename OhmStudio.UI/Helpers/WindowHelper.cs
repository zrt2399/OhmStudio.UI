using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace OhmStudio.UI.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    //https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-windowplacement
    public struct WindowPlacement
    {
        public int length;
        public int flags;
        public int showCmd;
        public RECT ptMinPosition;
        public RECT ptMaxPosition;
        public RECT rcNormalPosition;
    }

    // WINDOWPLACEMENT 结构体
    //https://learn.microsoft.com/zh-cn/windows/win32/api/windef/ns-windef-rect
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    // 定义必要的常量和结构体
    [StructLayout(LayoutKind.Sequential)]
    //https://learn.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-flashwinfo
    public struct FlashInfo
    {
        public uint cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public uint uCount;
        public uint dwTimeout;
    }

    public class WindowHelper
    {
        // 窗口显示命令常量
        public const int SwShownormal = 1;
        public const int SwShowminimized = 2;
        public const int SwShowmaximized = 3;

        public const uint FLASHW_STOP = 0;
        public const uint FLASHW_CAPTION = 1;
        public const uint FLASHW_TRAY = 2;
        public const uint FLASHW_ALL = 3;
        public const uint FLASHW_TIMER = 4;
        public const uint FLASHW_TIMERNOFG = 12;

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement windowPlacement);

        [DllImport("user32.dll")]
        public static extern bool FlashWindowEx(ref FlashInfo pwfi);

        public static bool FlashTaskbarIcon(IntPtr intPtr)
        {
            if (intPtr != IntPtr.Zero)
            {
                FlashInfo flashInfo = new FlashInfo
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(FlashInfo)),
                    hwnd = intPtr,
                    dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG,    // 闪烁模式，ALL表示图标和任务栏都闪烁
                    uCount = uint.MaxValue,  // 闪烁次数，使用最大值表示无限闪烁
                    dwTimeout = 0            // 闪烁的延迟，0表示默认值
                };

                return FlashWindowEx(ref flashInfo);
            }
            return false;
        }

        public static bool FlashTaskbarIcon(Window window)
        {
            return window != null && FlashTaskbarIcon(new WindowInteropHelper(window).Handle);
        }

        public static void ShowAndActivate(Process process)
        {
            if (process != null)
            {
                ShowAndActivate(process.MainWindowHandle);
            }
        }

        public static void ShowAndActivate(Window window)
        {
            if (window != null)
            {
                if (!window.IsVisible)
                {
                    window.Show();
                }
                ShowAndActivate(new WindowInteropHelper(window).Handle);
            }
        }

        public static void ShowAndActivate(IntPtr windowIntPtr)
        {
            if (windowIntPtr != IntPtr.Zero)
            {
                GetWindowPlacement(windowIntPtr, out var lpwndpl);
                var status = lpwndpl.showCmd == SwShowmaximized ? 3 : 1;
                ShowWindowAsync(windowIntPtr, status); //调用api函数，正常显示窗口，5：（保留状态）
                SetForegroundWindow(windowIntPtr);
            }
        }
    }
}