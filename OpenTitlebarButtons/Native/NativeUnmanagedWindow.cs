//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace OpenTitlebarButtons.Native
{
    public class NativeUnmanagedWindow : NativeWindow
    {
        public NativeUnmanagedWindow(IntPtr hWnd)
        {
            HandleRef = new HandleRef(this, hWnd);
        }

        public HandleRef HandleRef { get; set; }

        public Rectangle Bounds
        {
            get
            {
                User32_Gdi.GetWindowRect(HandleRef, out var rect);
                return rect;
            }
            set => User32_Gdi.SetWindowPos(HandleRef, (IntPtr) 0, value.X, value.Y, value.Width, value.Height,
                User32_Gdi.SetWindowPosFlags.SWP_NOZORDER);
        }

        public Point Location
        {
            get => Bounds.Location;
            set => User32_Gdi.SetWindowPos(HandleRef, (IntPtr) 0, value.X, value.Y, 0, 0,
                User32_Gdi.SetWindowPosFlags.SWP_NOSIZE | User32_Gdi.SetWindowPosFlags.SWP_NOZORDER);
        }

        public Size Size
        {
            get => Bounds.Size;
            set => User32_Gdi.SetWindowPos(HandleRef, (IntPtr) 0, 0, 0, value.Width, value.Height,
                User32_Gdi.SetWindowPosFlags.SWP_NOMOVE | User32_Gdi.SetWindowPosFlags.SWP_NOZORDER);
        }

        public int Left
        {
            get => Location.X;
            set
            {
                var l = Location;
                l.X = value;
                Location = l;
            }
        }

        public int Top
        {
            get => Location.Y;
            set
            {
                var l = Location;
                l.Y = value;
                Location = l;
            }
        }

        public int Right => Location.X + Size.Width;

        public int Bottom => Location.X + Size.Width;
        public event EventHandler<EventArgs> SizeChanged;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == /*WM_SIZE*/ 0x0005)
            {
                OnSizeChanged();
            }

            base.WndProc(ref m);
        }

        protected virtual void OnSizeChanged()
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public static explicit operator NativeUnmanagedWindow(Form f)
        {
            return new NativeUnmanagedWindow(f.Handle);
        }
    }
}