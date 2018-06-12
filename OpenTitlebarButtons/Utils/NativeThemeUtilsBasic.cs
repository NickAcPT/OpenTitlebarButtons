using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OpenTitlebarButtons.Enums;
using Vanara.PInvoke;
using Vanara.Windows.Forms;

namespace OpenTitlebarButtons.Utils
{
    public partial class NativeThemeUtils
    {
        private const string BasicClassListValue = "WINDOW";
        private const int TmtDibdata = 2;

        private static VisualTheme GetBasicWindowVisualTheme(IWin32Window w = null)
        {
            var theme = new VisualTheme(BasicClassListValue);
            if (w != null)
                theme.GetType().GetField("hTheme", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(theme, LoadBasicThemeData(w));
            return theme;
        }

        private static UxTheme.SafeThemeHandle LoadBasicThemeData(IWin32Window nWindow)
        {
            return UxTheme.OpenThemeDataEx(new HandleRef(nWindow, nWindow.Handle), BasicClassListValue,
                UxTheme.OpenThemeDataOptions.None);
        }

        public static Bitmap GetBasicWindowBitmap()
        {
            using (var w = CreateNativeWindow())
            {
                using (var hTheme = GetBasicWindowVisualTheme())
                {
                    //TODO: Use the orgiginal GetBitmap method
                    hTheme.GetBitmap((int) BasicTitlebarButtonPart.RestoreButton, (int) TitlebarButtonState.None,
                        TmtDibdata);
                }
            }

            return null;
        }
    }
}