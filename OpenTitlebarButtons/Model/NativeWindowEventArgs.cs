//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System;
using OpenTitlebarButtons.Native;

namespace OpenTitlebarButtons.Model
{
    public class NativeWindowEventArgs : EventArgs
    {
        public NativeUnmanagedWindow NativeWindow { get; set; }
    }
}