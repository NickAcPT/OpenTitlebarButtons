using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.Extensions;
using Vanara.PInvoke;
using Vanara.Windows.Forms;

namespace OpenTitlebarButtons.Utils
{
    public class NativeThemeUtils
    {
        private const string ClassListValue = "DWMWINDOW";

        private static Image Slice(Image original, Point loc, Size size)
        {
            var bmp = new Bitmap(size.Width, size.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(original, new Rectangle(Point.Empty, size), new Rectangle(loc, size));
            }

            return bmp;
        }


        private static RECT GetThemeRect(UxTheme.SafeThemeHandle hTheme, int iPartId, int iStateId, int iPropId)
        {
            UxTheme.GetThemeRect(hTheme, iPartId, iStateId, iPropId, out var rect);
            return rect;
        }

        public static Image GetDwmWindowButton(TitlebarButtonPart button, TitlebarButtonState state)
        {
            const int tmtAtlasrect = 8002;
            const int tmtImagecount = 2401;

            using (var vs = GetDwmWindowVisualTheme())
            {
                using (var hInstance = LoadAeroTheme())
                {
                    var nWindow = new NativeWindow();
                    nWindow.CreateHandle(new CreateParams());
                    Image result;
                    using (var theme = LoadAeroThemeData(nWindow))
                    {
                        var atlas = GetImageAtlasFromTheme(vs, hInstance);
                        if (atlas == null) return null;

                        var rect = GetThemeRect(theme, (int) button, (int) state, tmtAtlasrect);

                        result = Slice(atlas, rect.Location, rect.Size);


                        //Slice the buttons
                        UxTheme.GetThemeInt(theme, (int) button, (int) state, tmtImagecount, out var count);

                        var buttonSize = rect.Height / count;
                        
                        var startPoint = Point.Empty;
                        var btnSize = new Size(rect.Width, buttonSize);
                        startPoint.Offset(0, buttonSize * ((int) state - 1));

                        var buttonRect = new Rectangle(startPoint, btnSize);
                        buttonRect.Inflate(-1, -1);
                        result = Slice(result, buttonRect.Location, buttonRect.Size);

                    }
                    nWindow.DestroyHandle();
                    return result;
                }
            }

            return null;
        }


        public static Image GetDwmWindowAtlas()
        {
            using (var vs = GetDwmWindowVisualTheme())
            {
                using (var hInstance = LoadAeroTheme())
                {
                    return GetImageAtlasFromTheme(vs, hInstance);
                }
            }

            return null;
        }

        private static Image GetImageAtlasFromTheme(VisualTheme vs, Kernel32.SafeLibraryHandle hInstance)
        {
            var byteStream = vs.GetDiskStream(hInstance, 0, 0, 213);
            if (byteStream == null) return null;
            using (var ms = new MemoryStream(byteStream))
            {
                return Image.FromStream(ms);
            }
        }

        private static VisualTheme GetDwmWindowVisualTheme()
        {
            return new VisualTheme(ClassListValue);
        }

        private static Kernel32.SafeLibraryHandle LoadAeroTheme()
        {
            return Kernel32.LoadLibraryEx(
                @"C:\Windows\resources\themes\Aero\aero.msstyles",
                dwFlags: Kernel32.LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
        }

        private static UxTheme.SafeThemeHandle LoadAeroThemeData(NativeWindow nWindow)
        {
            return UxTheme.OpenThemeDataEx(new HandleRef(nWindow, nWindow.Handle), ClassListValue,
                UxTheme.OpenThemeDataOptions.None);
        }
    }
}