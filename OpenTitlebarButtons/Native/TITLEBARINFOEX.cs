//  
// Copyright (c) NickAc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//  

using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace OpenTitlebarButtons.Native
{
    public enum  StateSystemConstants	: uint
	   {
		StateSystemNone			=  0x00000000,
		StateSystemAlertHigh         	=  0x10000000,
		StateSystemAlertLow       		=  0x08000000,
		StateSystemAlertMedium          	=  0x04000000,
		StateSystemAnimated           	=  0x00004000,
		StateSystemBusy               	=  0x00000800,
		StateSystemChecked            	=  0x00000010,
		StateSystemCollapsed          	=  0x00000400,
		StateSystemDefault            	=  0x00000100,
		StateSystemExpanded           	=  0x00000200,
		StateSystemExtselectable      	=  0x02000000,
		StateSystemFloating           	=  0x00001000,
		StateSystemFocusable          	=  0x00100000,
		StateSystemFocused            	=  0x00000004,
		StateSystemHottracked         	=  0x00000080,
		StateSystemIndeterminate      	=  StateSystemMixed,
		StateSystemInvisible          	=  0x00008000,
		StateSystemLinked             	=  0x00400000,
		StateSystemMarqueed           	=  0x00002000,
		StateSystemMixed              	=  0x00000020,
		StateSystemMoveable           	=  0x00040000,
		StateSystemMultiselectable    	=  0x01000000,
		StateSystemOffscreen          	=  0x00010000,
		StateSystemPressed            	=  0x00000008,
		StateSystemProtected          	=  0x20000000,
		StateSystemReadonly           	=  0x00000040,
		StateSystemSelectable         	=  0x00200000,
		StateSystemSelected           	=  0x00000002,
		StateSystemSelfvoicing        	=  0x00080000,
		StateSystemSizeable           	=  0x00020000,
		StateSystemTraversed          	=  0x00800000,
		StateSystemUnavailable        	=  0x00000001,
		StateSystemValid              	=  0x3FFFFFFF
	    }
    public enum CchildrenTitlebarConstants
    {
        CchildrenTitlebarBar = 0,
        CchildrenTitlebarReserved = 1,
        CchildrenTitlebarMinimizeButton = 2,
        CchildrenTitlebarMaximizeButton = 3,
        CchildrenTitlebarHelpButton = 4,
        CchildrenTitlebarCloseButton = 5,
        CchildrenTitlebarMax = CchildrenTitlebarCloseButton
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Titlebarinfoex
    {
        public uint cbSize;
        public RECT rcTitleBar;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =
            (int) CchildrenTitlebarConstants.CchildrenTitlebarMax + 1)]
        public StateSystemConstants[] rgstate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =
            (int) CchildrenTitlebarConstants.CchildrenTitlebarMax + 1)]
        public RECT[] rgrect;

        public RECT GetRectangle(CchildrenTitlebarConstants btn)
        {
            return rgrect[(int) btn];
        }
    }
}