using CssUI.CSS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Represents the rendering bounds for a set of controls.
    /// </summary>
    public class cssViewport
    {
        #region Properties
        public Vec2i Origin { get; } = new Vec2i();
        public Size2D Size { get; } = new Size2D();
        public CssBoxArea Area { get; private set; } = new CssBoxArea();
        #endregion

        #region Events
        public event Action<Size2D, Size2D> Resized;
        public event Action<Vec2i, Vec2i> Moved;
        #endregion

        #region Constructors
        public cssViewport()
        {
        }
        #endregion

        public void Set(int X, int Y, int Width, int Height)
        {
            Origin.X = X;
            Origin.Y = Y;
            Size.Width = Width;
            Size.Height = Height;
            update_block();
        }

        public void Set_Size(int Width, int Height)
        {
            Size.Width = Width;
            Size.Height = Height;
            update_block();
        }

        private void update_block()
        {
            var old = Area;
            Area = new CssBoxArea();// XXX: this null may cause problems indeed
            Area.Update_Bounds(Origin.X, Origin.Y, Size.Width, Size.Height);

            if (old?.Get_Dimensions() != Area?.Get_Dimensions()) Resized?.Invoke(old.Get_Dimensions(), Area.Get_Dimensions());
            if (old.Get_Pos() != Area.Get_Pos()) Moved?.Invoke(old.Get_Pos(), Area.Get_Pos());
        }

    }
}
