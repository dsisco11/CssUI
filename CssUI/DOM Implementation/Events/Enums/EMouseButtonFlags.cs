﻿using System;

namespace CssUI.DOM.Events
{
    [Flags]
    public enum EMouseButtonFlags : ushort
    {
        None = 0x0,
        Left = 0x1,
        Right = 0x2,
        Middle = 0x4,
    }
}
