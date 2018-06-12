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
        protected override bool ShowWithoutActivation => true;

        public NativeUnmanagedWindow ParentWindow { get; }

        public TitlebarButtonHosterForm(NativeUnmanagedWindow parent)
        {
            ParentWindow = parent;
            Attach(parent);
            SetBitmap(NativeThemeUtils.GetDwmWindowButton(AeroTitlebarButtonPart.MinimizeButton,
                TitlebarButtonState.Hot) as Bitmap);
            Show();
        }

        private void Attach(NativeUnmanagedWindow parent)
        {
            NativeThemeUtils.SetWindowLong(Handle, NativeThemeUtils.GWLParameter.GWL_STYLE, parent.Handle.ToInt32());
            parent.WindowChanged += (s, e) => Relocate(this, parent);
            Relocate(this, parent);
        }

        private void Relocate(TitlebarButtonHosterForm frm, NativeUnmanagedWindow parent)
        {
            var loc = parent.Location;

            frm.Top = loc.Y;
            frm.Left = loc.X;
        }
    }
}