using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTitlebarButtons.Enums;
using OpenTitlebarButtons.Native;
using OpenTitlebarButtons.Utils;

namespace OpenTitlebarButtonsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            /*var w = new PerPixelAlphaWindow();
            if (NativeThemeUtils.GetDwmWindowButton(AeroTitlebarButtonPart.RestoreButton,
                TitlebarButtonState.Hot) is Bitmap bmp)
            {
                w.SetBitmap(bmp);
                Closing += (s, e) => w.Dispose();
                LocationChanged += (s, e) => RelocateWindow(w);
                SizeChanged += (s, e) => RelocateWindow(w);
                RelocateWindow(w);
                Show();
            }
            else
                w.Dispose();*/
            var frm = new TitlebarButtonHosterForm(new NativeUnmanagedWindow(new IntPtr(0x40A92)));
            frm.LocationChanged += (s, e) =>
            {
                Console.WriteLine(@"Location changed");
            };

        }

        private int _rightOffset = -1;
        private const int BtnsOffset = 10;

        private void RelocateWindow(PerPixelAlphaWindow w)
        {
            if (_rightOffset == -1)
            {
                var r = NativeThemeUtils.GetTitleBarInfoEx(Handle);
                var rectBtn = r.GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarMinimizeButton);
                var rectTbl = r.rcTitleBar;
                _rightOffset = rectTbl.Width - (rectTbl.right - rectBtn.left);
            }

            w.Left = Left + Width - BtnsOffset - _rightOffset;
            w.Top = Top;
        }
    }
}