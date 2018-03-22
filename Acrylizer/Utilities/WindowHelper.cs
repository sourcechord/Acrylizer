using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Acrylizer.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    class WindowInfo
    {
        public IntPtr Handle;
        public string ClassName;
        public string Text;
        public RECT Rect;

        public WindowInfo(IntPtr handle, string className, string text, RECT rect)
        {
            this.Handle = handle;
            this.ClassName = className;
            this.Text = text;
            this.Rect = rect;
        }
    }

    class WindowHelper
    {
        public const int WM_GETTEXT = 0xD;
        public const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr handle, StringBuilder className, int maxCount);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr handle, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr handle, int msg, int wParam, System.Text.StringBuilder lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr handle, out RECT Rect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref POINT pt);

        public static string GetWindowClassName(IntPtr handle)
        {
            StringBuilder buffer = new StringBuilder(128);
            GetClassName(handle, buffer, buffer.Capacity);
            return buffer.ToString();
        }

        public static string GetWindowText(IntPtr handle)
        {
            StringBuilder buffer = new StringBuilder(SendMessage(handle, WM_GETTEXTLENGTH, 0, 0) + 1);
            SendMessage(handle, WM_GETTEXT, buffer.Capacity, buffer);
            return buffer.ToString();
        }

        public static RECT GetWindowRectangle(IntPtr handle)
        {
            RECT rect = new RECT();
            GetWindowRect(handle, out rect);
            return rect;
        }


        public static WindowInfo GetWindowInfo()
        {
            POINT pt = new POINT();
            GetCursorPos(ref pt);
            return GetWindowInfo(pt);
        }

        public static WindowInfo GetWindowInfo(POINT point)
        {
            var handle = WindowFromPoint(point);
            var className = GetWindowClassName(handle);
            var text = GetWindowText(handle);
            var rect = GetWindowRectangle(handle);

            return new WindowInfo(handle, className, text, rect);
        }

    }
}
