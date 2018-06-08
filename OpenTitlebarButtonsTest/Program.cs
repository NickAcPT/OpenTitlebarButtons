using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTitlebarButtons;
using OpenTitlebarButtons.Utils;

namespace OpenTitlebarButtonsTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //var atlas = NativeThemeUtils.GetDwmWindowAtlas();
            //atlas?.Save("atlas.png", ImageFormat.Png);

            NativeThemeUtils.GetDwmWindowButton(TitlebarButtonPart.RestoreButton, TitlebarButtonState.Normal)?.Save("btn.png", ImageFormat.Png);

            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
        }
    }
}