using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace TFTCalculator
{
    // See: https://stackoverflow.com/questions/36725769/wpf-windowchrome-edges-of-maximized-window-are-out-of-the-screen/61299413#61299413
    public static class SystemParametersFix
    {
        public static Thickness WindowResizeBorderThickness
        {
            get
            {
                float dpix = GetDpi(GetDeviceCapsIndex.LOGPIXELSX);
                float dpiy = GetDpi(GetDeviceCapsIndex.LOGPIXELSY);

                int dx = GetSystemMetrics(GetSystemMetricsIndex.CXFRAME);
                int dy = GetSystemMetrics(GetSystemMetricsIndex.CYFRAME);

                // this adjustment is needed only since .NET 4.5 
                int d = GetSystemMetrics(GetSystemMetricsIndex.SM_CXPADDEDBORDER);
                dx += d;
                dy += d;

                var leftBorder = dx / dpix;
                var topBorder = dy / dpiy;

                return new Thickness(leftBorder, topBorder, leftBorder, topBorder);
            }
        }

        private enum GetDeviceCapsIndex
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }


        private enum GetSystemMetricsIndex
        {
            CXFRAME = 32,
            CYFRAME = 33,
            SM_CXPADDEDBORDER = 92
        }


        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(GetSystemMetricsIndex nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        private static float GetDpi(GetDeviceCapsIndex index)
        {
            IntPtr desktopWnd = IntPtr.Zero;
            IntPtr dc = GetDC(desktopWnd);
            float dpi;
            try
            {
                dpi = GetDeviceCaps(dc, (int)index);
            }
            finally
            {
                ReleaseDC(desktopWnd, dc);
            }
            return dpi / 96f;
        }
    }
}
