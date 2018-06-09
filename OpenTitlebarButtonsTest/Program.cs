using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using OpenTitlebarButtons;
using OpenTitlebarButtons.Native;
using OpenTitlebarButtons.Utils;

namespace OpenTitlebarButtonsTest
{
    static class Program
    {
        static TaskCompletionSource<object> _showPrm;

    [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*
            NativeThemeUtils.GetBasicWindowBitmap()?.Save("testbasic.png", ImageFormat.Png);

            var frm = new Form1
            {
                StartPosition = FormStartPosition.CenterScreen
            };


            frm.Show();
            var hasPainted = false;
            _showPrm = new TaskCompletionSource<object>();
            frm.Paint += (s, e) =>
            {
                if (hasPainted) return;
                hasPainted = true;
                _showPrm.TrySetResult(new object());
            };

            Application.DoEvents();

            _showPrm.Task.GetAwaiter().GetResult();

            var sb = new StringBuilder();

            sb.AppendLine("Environment.OSVersion.Version.ToString(): " + Environment.OSVersion.Version.ToString());


            var result = NativeThemeUtils.GetTitleBarInfoEx(frm.Handle, out var pti);

            sb.AppendLine("TitlebarInfoEx: " + JsonConvert.SerializeObject(pti));

            sb.AppendLine("frm.Location: " + frm.Location.ToString());

            var screenBitmap = GetScreenShot(pti);
            screenBitmap.Save("scr_bmp.bmp");



            Console.WriteLine(sb.ToString());

            Console.ReadKey();

            frm.Dispose();
            */
            Application.Run(new Form1
            {
                StartPosition = FormStartPosition.CenterScreen
            });

        }


        private static Bitmap GetScreenShot(Titlebarinfoex pti)
        {
            var bmpScreenshot = new Bitmap(pti.rcTitleBar.Width,
                pti.rcTitleBar.Height,
                PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            using (var g = Graphics.FromImage(bmpScreenshot))
            {
                g.CopyFromScreen(pti.rcTitleBar.X,
                    pti.rcTitleBar.Y,
                    0,
                    0,
                    pti.rcTitleBar.Size,
                    CopyPixelOperation.SourceCopy);
                var scr = Screen.FromPoint(Cursor.Position);
                using (var bmp2 = new Bitmap(scr.Bounds.Width, scr.Bounds.Height))
                {
                    using (var g2 = Graphics.FromImage(bmp2))
                    {
                        g2.FillRectangle(Brushes.Red,
                            pti.rgrect[(int) CchildrenTitlebarConstants.CchildrenTitlebarMinimizeButton]);
                    }

                    g.DrawImage(bmp2, new Rectangle(0, 0, pti.rcTitleBar.Width, pti.rcTitleBar.Height),
                        new Rectangle(pti.rcTitleBar.X, pti.rcTitleBar.Y, pti.rcTitleBar.Width, pti.rcTitleBar.Height),
                        GraphicsUnit.Pixel);
                }
            }

            return bmpScreenshot;
        }
    }


    ///// <summary>
    ///// The main entry point for the application.
    ///// </summary>
    //[STAThread]
    //static void Main()
    //{
    //    //var atlas = NativeThemeUtils.GetDwmWindowAtlas();
    //    //atlas?.Save("atlas.png", ImageFormat.Png);

    //    NativeThemeUtils.GetDwmWindowButton(TitlebarButtonPart.MinimizeRestoreButtonGlow, TitlebarButtonState.None)?.Save("btnglow.png", ImageFormat.Png);

    //    /*
    //    Application.EnableVisualStyles();
    //    Application.SetCompatibleTextRenderingDefault(false);
    //    Application.Run(new Form1());*/
    //}
}