using System.Runtime.InteropServices;
using System.Windows.Input;

namespace OhmStudio.UI.Helpers
{
    public static class KeyboardHelper
    {
        //https://learn.microsoft.com/zh-cn/windows/win32/inputdev/virtual-key-codes
        [DllImport("user32.dll")]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public static bool IsCtrlKeyDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

        public static bool IsShiftKeyDown => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

        public static bool IsAltKeyDown => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

        public static void KeyboardDown(byte keyCode)
        {
            keybd_event(keyCode, 0, 0, 0);// 模拟按下键
        }

        public static void KeyboardUp(byte keyCode)
        {
            keybd_event(keyCode, 0, 2, 0);// 模拟释放键
        }
    }
}