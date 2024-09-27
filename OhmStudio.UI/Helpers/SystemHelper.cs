using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace OhmStudio.UI.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    public class SystemHelper
    {
        private const string SkinTypeRegistryKeyName = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        private const string SkinTypeRegistryValueName = "AppsUseLightTheme";

        [DllImport("kernel32.dll")]
        internal static extern ulong GetTickCount64();

        [DllImport("user32.dll")]
        internal static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static ulong TickCount64 => GetTickCount64();

        public static ulong IdleTimeInMilliseconds
        {
            get
            {
                LASTINPUTINFO lASTINPUTINFO = new LASTINPUTINFO();
                lASTINPUTINFO.cbSize = (uint)Marshal.SizeOf(lASTINPUTINFO);
                return GetLastInputInfo(ref lASTINPUTINFO) ? TickCount64 - lASTINPUTINFO.dwTime : 0;
            }
        }

        public static double IdleTimeInSeconds => IdleTimeInMilliseconds / 1000d;

        public static double IdleTimeInMinutes => IdleTimeInSeconds / 60;

        public static double IdleTimeInHours => IdleTimeInMinutes / 60;

        public static double IdleTimeInDays => IdleTimeInHours / 24;

        public static bool DetermineIfInLightThemeMode
        {
            get
            {
                object value = Registry.GetValue(SkinTypeRegistryKeyName, SkinTypeRegistryValueName, "0");
                if (value != null)
                {
                    return value.ToString() != "0";
                }

                return false;
            }
        }
    }
}