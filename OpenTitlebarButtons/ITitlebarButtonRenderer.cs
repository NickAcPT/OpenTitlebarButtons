//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System.Drawing;

namespace OpenTitlebarButtons
{
    public interface ITitlebarButtonRenderer
    {
        void RenderButton(Graphics g, TitlebarButtonType type, TitlebarButtonState state);
    }
}