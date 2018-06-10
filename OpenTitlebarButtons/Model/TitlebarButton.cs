//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System.Drawing;
using OpenTitlebarButtons.Enums;
using OpenTitlebarButtons.Native;
using OpenTitlebarButtons.Utils;

namespace OpenTitlebarButtons.Model
{
    public class TitlebarButton : TitlebarItemBase
    {
        private Size _innerSize = QueryTitlebarButtonSize();

        public TitlebarButtonState ButtonState { get; set; }

        public Bitmap Icon { get; set; }

        private static Size QueryTitlebarButtonSize()
        {
            return NativeThemeUtils.GetTitleBarInfoEx()
                .GetRectangle(CchildrenTitlebarConstants.CchildrenTitlebarMinimizeButton).Size;
        }

        protected override void OnThemeUpdated()
        {
            base.OnThemeUpdated();
            _innerSize = QueryTitlebarButtonSize();
        }
    }
}