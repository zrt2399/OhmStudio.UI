﻿using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace OhmStudio.UI.Helpers
{
    public static class MouseHelper
    {
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern unsafe IntPtr GetCursorPos(System.Drawing.Point* lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct PointWrap
        {
            public int X;
            public int Y;
        }

        public static Point GetMousePosition()
        {
            static bool GetPointWrap(out PointWrap point)
            {
                unsafe
                {
                    fixed (PointWrap* ptr = &point)
                    {
                        return GetCursorPos((System.Drawing.Point*)ptr) != IntPtr.Zero;
                    }
                }
            }

            if (GetPointWrap(out var point))
            {
                return new Point(point.X, point.Y);
            }

            return new Point();
        }
    }
}