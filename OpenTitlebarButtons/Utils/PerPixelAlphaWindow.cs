//  
// Copyright © 2002 Rui Godinho Lopes
//  

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace OpenTitlebarButtons.Utils
{
// a static class to expose needed win32 gdi functions.
    class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public int cx;
            public int cy;

            public Size(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private struct Argb
        {
            public readonly byte Blue;
            public readonly byte Green;
            public readonly byte Red;
            public readonly byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Blendfunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        public const int UlwAlpha = 0x00000002;

        public const byte AcSrcOver = 0x00;
        public const byte AcSrcAlpha = 0x01;


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize,
            IntPtr hdcSrc, ref Point pprSrc, int crKey, ref Blendfunction pblend, int dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);
    }


    /// <para>PerPixel forms should derive from this base class</para>
    /// <author><name>Rui Godinho Lopes</name><email>rui@ruilopes.com</email></author>
    public class PerPixelAlphaWindow : Form
    {
        /// <para>Changes the current bitmap.</para>
        public void SetBitmap(Bitmap bitmap)
        {
            SetBitmap(bitmap, 255);
        }

        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            var screenDc = Win32.GetDC(IntPtr.Zero);
            using (var memDc = new Gdi32.SafeDCHandle(Gdi32.CreateCompatibleDC(screenDc)))
            {
                var hBitmap = IntPtr.Zero;
                var oldBitmap = IntPtr.Zero;

                try
                {
                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0)); // grab a GDI handle from this GDI+ bitmap
                    oldBitmap = Gdi32.SelectObject(memDc, hBitmap);

                    var size = new Win32.Size(bitmap.Width, bitmap.Height);
                    var pointSource = new Win32.Point(0, 0);
                    var topPos = new Win32.Point(Left, Top);
                    var blend = new Win32.Blendfunction
                    {
                        BlendOp = Win32.AcSrcOver,
                        BlendFlags = 0,
                        SourceConstantAlpha = opacity,
                        AlphaFormat = Win32.AcSrcAlpha
                    };

                    Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc.DangerousGetHandle(),
                        ref pointSource, 0, ref blend,
                        Win32.UlwAlpha);
                }
                finally
                {
                    Win32.ReleaseDC(IntPtr.Zero, screenDc);
                    if (hBitmap != IntPtr.Zero)
                    {
                        Gdi32.SelectObject(memDc, oldBitmap);
                        //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                        Gdi32.DeleteObject(hBitmap);
                    }
                }
            }
        }


        protected override CreateParams CreateParams
        {
            get
            {
                var cp = new CreateParams();
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }
    }
}