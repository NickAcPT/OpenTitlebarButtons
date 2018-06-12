//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System;
using System.Drawing;
using OpenTitlebarButtons.Native;

namespace OpenTitlebarButtons.Utils
{
    public class ManagedTitlebarInfoEx
    {
        public class TitlebarElement
        {
            public TitlebarElement(TitlebarElementKind kind, SystemState state, Rectangle rectangle)
            {
                Kind = kind;
                State = state;
                Rectangle = rectangle;
            }

            public TitlebarElementKind Kind { get; set; }
            public SystemState State { get; set; }
            public Rectangle Rectangle { get; set; }
        }

        public enum TitlebarElementKind
        {
            TitlebarBar = 0,
            MinimizeButton = 2,
            MaximizeButton = 3,
            HelpButton = 4,
            CloseButton = 5
        }

        public enum SystemState
        {
            Focusable =
            1048576,

            Invisible =
            32768,

            Offscreen =
            65536,

            Unavailable =
            1,

            Pressed =
            8
        }

        /*        CchildrenTitlebarMinimizeButton = 2,
        CchildrenTitlebarMaximizeButton = 3,
        CchildrenTitlebarHelpButton = 4,
        CchildrenTitlebarCloseButton = 5,*/

        public TitlebarElement HelpButton { get; set; }

        public TitlebarElement CloseButton { get; set; }

        public TitlebarElement MaximizeButton { get; set; }

        public TitlebarElement MinimizeButton { get; set; }

        public static explicit operator ManagedTitlebarInfoEx(Titlebarinfoex info)
        {
            var managedInfo = new ManagedTitlebarInfoEx
            {
                CloseButton = new TitlebarElement(TitlebarElementKind.CloseButton,
                    (SystemState) Enum.ToObject(typeof(SystemState),
                        info.GetState(CchildrenTitlebarConstants.CchildrenTitlebarCloseButton)),
                    info.GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarCloseButton)),
                HelpButton = new TitlebarElement(TitlebarElementKind.HelpButton,
                    (SystemState) Enum.ToObject(typeof(SystemState),
                        info.GetState(CchildrenTitlebarConstants.CchildrenTitlebarHelpButton)),
                    info.GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarHelpButton)),
                MaximizeButton = new TitlebarElement(TitlebarElementKind.MaximizeButton,
                    (SystemState) Enum.ToObject(typeof(SystemState),
                        info.GetState(CchildrenTitlebarConstants.CchildrenTitlebarMaximizeButton)),
                    info.GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarMaximizeButton)),
                MinimizeButton = new TitlebarElement(TitlebarElementKind.MinimizeButton,
                    (SystemState) Enum.ToObject(typeof(SystemState),
                        info.GetState(CchildrenTitlebarConstants.CchildrenTitlebarMinimizeButton)),
                    info.GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarMinimizeButton))
            };
            return managedInfo;
        }
    }
}