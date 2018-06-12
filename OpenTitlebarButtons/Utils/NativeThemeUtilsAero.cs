using System.Drawing;
using System.IO;
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
        private const int WmGettitlebarinfoex = 0x033F;

        private const string DwmClassListValue = "DWMWINDOW";


        private static RECT GetThemeRect(UxTheme.SafeThemeHandle hTheme, int iPartId, int iStateId, int iPropId)
        {
            UxTheme.GetThemeRect(hTheme, iPartId, iStateId, iPropId, out var rect);
            return rect;
        }


        public static Image GetDwmWindowButton(AeroTitlebarButtonPart button, TitlebarButtonState state)
        {
            const int tmtAtlasrect = 8002;
            const int tmtImagecount = 2401;

            using (var vs = GetDwmWindowVisualTheme())
            {
                using (var hInstance = LoadAeroTheme())
                {
                    Image result;
                    using (var nWindow = CreateNativeWindow())
                    {
                        using (var theme = LoadAeroThemeData(nWindow))
                        {
                            var atlas = GetImageAtlasFromTheme(vs, hInstance);
                            if (atlas == null) return null;

                            var rect = GetThemeRect(theme, (int) button, (int) state, tmtAtlasrect);

                            result = Slice(atlas, rect.Location, rect.Size);
                            if (state != TitlebarButtonState.None)
                            {
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
                        }
                    }

                    return result;
                }
            }
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
            return new VisualTheme(DwmClassListValue);
        }

        private static Kernel32.SafeLibraryHandle LoadAeroTheme()
        {
            return Kernel32.LoadLibraryEx(
                Path.Combine(GetWindowsFolder(), "resources", "themes", "Aero", "aero.msstyles"),
                dwFlags: Kernel32.LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
        }

        private static UxTheme.SafeThemeHandle LoadAeroThemeData(NativeWindow nWindow)
        {
            return UxTheme.OpenThemeDataEx(new HandleRef(nWindow, nWindow.Handle), DwmClassListValue,
                UxTheme.OpenThemeDataOptions.None);
        }

        public static string GetCurrentThemePath()
        {
            var sbFilename = new StringBuilder(0x200);
            var s = UxTheme.GetCurrentThemeName(sbFilename, 0x200, null, 0, null, 0);
            return sbFilename.ToString();
        }

        public static string GetCurrentThemeFolder()
        {
            var themePath = GetCurrentThemePath();
            return Path.GetDirectoryName(themePath);
        }

        public static string GetThemesFolder()
        {
            return Directory.GetParent(GetCurrentThemeFolder()).FullName;
        }
    }
}