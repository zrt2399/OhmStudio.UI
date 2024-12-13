﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop; 

namespace OhmStudio.UI.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public class WindowHelper
    {
        // 窗口显示命令常量
        public const int SwShownormal = 1;
        public const int SwShowminimized = 2;
        public const int SwShowmaximized = 3;

        [DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement windowPlacement);

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