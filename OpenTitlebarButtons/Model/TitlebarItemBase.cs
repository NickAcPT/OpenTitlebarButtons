//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System;
using OpenTitlebarButtons.Native;

namespace OpenTitlebarButtons.Model
{
    public class TitlebarItemBase
    {
        public int ItemIndex { get; set; }

        protected virtual int Width { get; } = 0;

        public event EventHandler<NativeWindowEventArgs> Click;

        public event EventHandler<EventArgs> ThemeUpdated;

        protected virtual void OnClick(NativeUnmanagedWindow w)
        {
            Click?.Invoke(this, new NativeWindowEventArgs {NativeWindow = w});
        }

        protected virtual void OnThemeUpdated()
        {
            ThemeUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}