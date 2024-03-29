﻿using CssUI.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Holds and manages all of the stacking values for an <see cref="IRenderEngine"/> implementation.
    /// </summary>
    public class RenderStack
    {
        #region Stacks
        Stack<Matrix4> MatrixStack = new Stack<Matrix4>();
        Stack<Color> ColorStack = new Stack<Color>();
        Stack<Color> BlendingStack = new Stack<Color>();
        #endregion

        #region Properties
        public Color Color => ColorStack.Peek() ?? Color.White;
        public Color Blend_Color => BlendingStack.Peek() ?? Color.White;
        public Matrix4 Matrix => MatrixStack.FirstOrDefault();
        #endregion

        #region Events
        public event Action Stack_Changed;
        #endregion


        #region Setting
        /// <summary>
        /// Sets the current color for the most recent stack item (this color is NOT blended by the stack)
        /// </summary>
        /// <param name="value"></param>
        public void Set_Color(Color value, bool FireChangedEvent = true)
        {
            // Remove old value from the top of the stack
            if (ColorStack.Count > 0) ColorStack.Pop();
            // Push the new value onto the stack
            ColorStack.Push(value);
            if (FireChangedEvent)
            {
                Stack_Changed?.Invoke();
            }
        }

        /// <summary>
        /// Sets the current blend color for the most recent stack item (this color is blended with all the blending colors before it)
        /// </summary>
        /// <param name="value"></param>
        public void Set_Blend(ReadOnlyColor value, bool FireChangedEvent = true)
        {            
            // Remove old value from the top of the stack
            if (BlendingStack.Count > 0) BlendingStack.Pop();
            // Find the value we need to stack the incoming one ontop of (so we can combine them)
            Color clr = BlendingStack.FirstOrDefault();
            // Combine the incoming value with the one before it(if any) in the stack
            if (clr != null) clr *= value;
            else clr = new Color(value);

            BlendingStack.Push(clr);
            if (FireChangedEvent)
            {
                Stack_Changed?.Invoke();
            }
        }

        public void Set_Matrix(Matrix4 value, bool FireChangedEvent = true)
        {
            // Remove the current matrix value from the stack so we can replace it
            if (MatrixStack.Count > 0) MatrixStack.Pop();
            // Find the value we need to stack the incoming one ontop of (so we can combine them)
            Matrix4 mtx = MatrixStack.FirstOrDefault();
            // Combine the incoming value with the one before it(if any) in the stack
            if (mtx != null) mtx *= value;
            else mtx = value;

            MatrixStack.Push(mtx);
            if (FireChangedEvent)
            {
                Stack_Changed?.Invoke();
            }
        }
        #endregion

        #region Pushing
        /// <summary>
        /// Push a new instance onto each of our stacks
        /// </summary>
        public void Push_All()
        {
            if (ColorStack.Count > 0) ColorStack.Push(ColorStack.First());
            else ColorStack.Push(null);

            if (BlendingStack.Count > 0) BlendingStack.Push(BlendingStack.First());
            else BlendingStack.Push(null);

            if (MatrixStack.Count > 0) MatrixStack.Push(MatrixStack.First());
            else MatrixStack.Push(null);
            
            Stack_Changed?.Invoke();
        }
        #endregion

        #region Popping
        /// <summary>
        /// Pop the last instance off of all our stacks
        /// </summary>
        public void Pop_All()
        {
            ColorStack.Pop();
            BlendingStack.Pop();
            MatrixStack.Pop();
            Stack_Changed?.Invoke();
        }
        #endregion
        
    }
}
