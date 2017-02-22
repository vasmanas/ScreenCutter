using System;

namespace ScreenCutter.App
{
    [Flags]
    public enum ExpandDirection
    {
        Top = 1,

        Bottom = 2,

        Left = 4,

        Right = 8,

        Vertical = Top | Bottom,

        Horizontal = Left | Right,

        All = Vertical | Horizontal,
    }
}
