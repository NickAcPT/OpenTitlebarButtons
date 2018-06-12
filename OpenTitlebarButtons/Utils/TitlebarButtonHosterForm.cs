using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTitlebarButtons.Enums;
using OpenTitlebarButtons.Native;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32_Gdi;

namespace OpenTitlebarButtons.Utils
{
    public class TitlebarButtonHosterForm : PerPixelAlphaWindow
    {
        [DllImport(Lib.User32, SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        protected override bool ShowWithoutActivation => true;

        public NativeUnmanagedWindow ParentWindow { get; }

        public TitlebarButtonHosterForm(NativeUnmanagedWindow parent)
        {
            ParentWindow = parent;
            Attach(parent);
            SetBitmap(NativeThemeUtils.GetDwmWindowButton(AeroTitlebarButtonPart.MinimizeButton,
                TitlebarButtonState.Hot) as Bitmap);
            Show(NativeWindow.FromHandle(parent.Handle));
            Console.WriteLine("ctor7");
        }

        private void Attach(NativeUnmanagedWindow parent)
        {
            parent.WindowChanged += (s, e) => Relocate(this, parent);
            Relocate(this, parent);
            SetWindowPos(new HandleRef(this, Handle), parent.Handle, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);
            NativeThemeUtils.SetWindowLong(Handle, NativeThemeUtils.GWLParameter.GWL_HWNDPARENT, parent.Handle.ToInt32());
        
        }

        private void Relocate(TitlebarButtonHosterForm frm, NativeUnmanagedWindow parent)
        {
            var loc = parent.Location;

            frm.Top = loc.Y;
            frm.Left = loc.X;
        }
    }
}