using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTitlebarButtons.Utils
{
    public class SafeNativeWindowHandle : Vanara.InteropServices.GenericSafeHandle
    {
        private static readonly Dictionary<IntPtr, NativeWindow> ToFree = new Dictionary<IntPtr, NativeWindow>();
        public NativeWindow NativeWindow { get; set; }
        public SafeNativeWindowHandle(NativeWindow w = null) : base(DestroyHandle)
        {
            NativeWindow = w ?? NativeThemeUtils.CreateNativeWindow();
            SetHandle(NativeWindow.Handle);
            ToFree.Add(NativeWindow.Handle, NativeWindow);
        }

        public static bool DestroyHandle(IntPtr ptr)
        {
            if (!ToFree.ContainsKey(ptr)) return false;
            var window = ToFree[ptr];
            window.DestroyHandle();
            return true;
        }
        
        public static implicit operator NativeWindow(SafeNativeWindowHandle h)
        {
            return h.NativeWindow;
        }
        
        public static implicit operator SafeNativeWindowHandle(NativeWindow w)
        {
            return new SafeNativeWindowHandle(w);
        }
        
    }
}
