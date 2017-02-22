using System.Runtime.InteropServices;

namespace ScreenCutter.App.Native
{
    /// <summary>
    /// References all of the Native Windows API methods for the WindowsInput functionality.
    /// </summary>
    internal static class WinApiMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);
    }
}
