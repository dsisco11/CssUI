using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Represents the rendering bounds for a set of controls.
    /// </summary>
    public class Viewport
    {
        #region Properties
        public ePos Origin { get; } = new ePos();
        public eSize Size { get; } = new eSize();
        public eBlock Block { get; private set; } = new eBlock();
        #endregion

        #region Events
        public event Action<eSize, eSize> Resized;
        public event Action<ePos, ePos> Moved;
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

        void update_block()
        {
            var old = Block;
            Block = new eBlock(Origin, Size);

            if (old.Get_Size() != Block.Get_Size()) Resized?.Invoke(old.Get_Size(), Block.Get_Size());
            if (old.Get_Pos() != Block.Get_Pos()) Moved?.Invoke(old.Get_Pos(), Block.Get_Pos());
        }

        Stack<eBlock> ScissorStack = new Stack<eBlock>(0);
        public void Push_Scissor(eBlock b)
        {
            // Reverse the orientation of our clipping area as OpenGL specifys it's viewport's origin location as the LOWER left corner not the upper left one.
            var area = Block_Flip_Y(b);
            ScissorStack.Push(area);
            /*GL.Enable(EnableCap.ScissorTest);
            GL.Scissor(area.X, area.Y, area.Width, area.Height);*/
        }

        eBlock Block_Flip_Y(eBlock b)
        {
            int y = (Block.Height - b.Y - b.Height);
            return eBlock.FromTRBL(y, b.Right, y+b.Height, b.Left);
        }
        /// <summary>
        /// Pops the current scissor area out off the stack and sets the opengl scissoring area to what is was previously
        /// </summary>
        public eBlock Pop_Scissor()
        {
            if (ScissorStack.Count == 0) throw new Exception("Scissor stack contains no more items!");

            eBlock block = Block_Flip_Y(ScissorStack.Pop());
            if (ScissorStack.Count == 0)
            {
                /*GL.Disable(EnableCap.ScissorTest);*/
                return block;
            }

            eBlock area = ScissorStack.First();
            /*GL.Scissor(area.X, area.Y, area.Width, area.Height);*/
            return block;
        }

        public void Scissor_Safety_Check()
        {
            if (ScissorStack.Count > 0) throw new Exception("Scissor stack not empty, a call to Pop_Scissor() is missing somewhere!");
        }

    }
}
