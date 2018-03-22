using Acrylizer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Acrylizer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        WindowInfo lastWindow = null;
        WindowInfo currentWindow = null;

        private bool isDrag = false;

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            this.Cursor = Cursors.Cross;
            this.isDrag = true;
            element?.CaptureMouse();
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            this.Cursor = Cursors.Arrow;
            this.isDrag = false;
            element?.ReleaseMouseCapture();

            // アクリル化を行う
            EnableBlur(this.lastWindow.Handle);

            var rect = ToRectangle(this.lastWindow.Rect);
            System.Windows.Forms.ControlPaint.DrawReversibleFrame(rect, System.Drawing.Color.Black, System.Windows.Forms.FrameStyle.Thick);
            this.lastWindow = null;
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isDrag) return;

            this.currentWindow = WindowHelper.GetWindowInfo();

            if (this.lastWindow == null)
            {
                // 描画処理
                var rect = ToRectangle(this.currentWindow.Rect);
                System.Windows.Forms.ControlPaint.DrawReversibleFrame(rect, System.Drawing.Color.Black, System.Windows.Forms.FrameStyle.Thick);
            }
            else if (!this.currentWindow.Handle.Equals(this.lastWindow.Handle))
            {
                var last = ToRectangle(this.lastWindow.Rect);
                System.Windows.Forms.ControlPaint.DrawReversibleFrame(last, System.Drawing.Color.Black, System.Windows.Forms.FrameStyle.Thick);

                var current = ToRectangle(this.currentWindow.Rect);
                System.Windows.Forms.ControlPaint.DrawReversibleFrame(current, System.Drawing.Color.Black, System.Windows.Forms.FrameStyle.Thick);
            }

            this.lastWindow = this.currentWindow;
        }


        private static System.Drawing.Rectangle ToRectangle(RECT src)
        {
            return new System.Drawing.Rectangle(src.Left, src.Top, (src.Right - src.Left) + 1, (src.Bottom - src.Top) + 1);
        }

        #region for AcrylicEffect
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public uint GradientColor;
            public int AnimationId;
        }


        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);


        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        //public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        internal void EnableBlur(IntPtr handle)
        {
            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            accent.AccentFlags = 2;
            var opacity = (byte)(this.sldOpacity.Value * 255);
            var col = HSVColor.FromHSV((float)this.hsvSelector.Hue, 1, 1).ToRGB();
            var color = (opacity << 24) + (col.B << 16) + (col.G << 8) + col.R;
            accent.GradientColor = (uint)color;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        #endregion
    }
}
