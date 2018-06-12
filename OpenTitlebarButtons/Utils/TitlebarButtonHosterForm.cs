using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OpenTitlebarButtons.Enums;
using OpenTitlebarButtons.Native;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32_Gdi;

namespace OpenTitlebarButtons.Utils
{
    public class TitlebarButtonHosterForm : PerPixelAlphaWindow
    {
        private const int WmMouseactivate = 0x0021, MaNoactivate = 0x0003;

        private HandleRef _hwndRef;

        public TitlebarButtonHosterForm(NativeUnmanagedWindow parent)
        {
            AutoScaleMode = AutoScaleMode.None;
            ParentWindow = parent;
            Show(NativeWindow.FromHandle(parent.Handle));
            Attach(parent);
            SetBitmap(NativeThemeUtils.GetDwmWindowButton(AeroTitlebarButtonPart.MinimizeButton,
                TitlebarButtonState.Hot) as Bitmap);
        }

        protected override bool ShowWithoutActivation => true;

        public NativeUnmanagedWindow ParentWindow { get; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmMouseactivate)
            {
                m.Result = (IntPtr) MaNoactivate;
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _hwndRef = new HandleRef(this, Handle);
        }

        private void Attach(NativeUnmanagedWindow parent)
        {
            parent.WindowChanged += (s, e) => Relocate(this, parent);
            SetWindowPos(new HandleRef(this, Handle), parent.Handle, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);
            NativeThemeUtils.SetWindowLong(Handle, NativeThemeUtils.GWLParameter.GWL_HWNDPARENT,
                parent.Handle.ToInt32());
            Relocate(this, parent);
        }

        private void Relocate(TitlebarButtonHosterForm frm, NativeUnmanagedWindow parent)
        {
            var loc = parent.Location;

            SetWindowPos(_hwndRef, (IntPtr) 0, loc.X, loc.Y, 0, 0,
                SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOSIZE);
        }
    }
}