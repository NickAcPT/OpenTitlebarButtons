//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Vanara.PInvoke;
using Vanara.Windows.Forms;

namespace OpenTitlebarButtons.Utils
{
    public static class VisualThemeExtensions
    {
        public static UxTheme.SafeThemeHandle GetTheme(this VisualTheme th)
        {
            return th.GetType().GetField("hTheme", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(th) as UxTheme.SafeThemeHandle;
        }

        
        [PInvokeData("UxTheme.h")]
        [DllImport("uxtheme.dll")]
        private static extern HRESULT GetThemeBitmap(UxTheme.SafeThemeHandle hTheme, int iPartId, int iStateId, int iPropId, int dwFlags, out IntPtr phBitmap);

        public static Bitmap GetBitmap(this VisualTheme th, int partId, int stateId, int propId)
        {
            return GetThemeBitmap(th.GetTheme(), partId, stateId, propId, 1, out var phBitmap).Succeeded ? Image.FromHbitmap(phBitmap) : null;
        }
    }
}