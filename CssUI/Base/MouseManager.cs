using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Provides access to ui state related tracking for the mouse
    /// </summary>
    public class MouseManager
    {
        #region Accessors
        /// <summary>
        /// Indicates whether the mouse is currently dragging an element or not
        /// </summary>
        public bool IsDragging { get { return (Dragging_Target != null); } }
        /// <summary>
        /// The element currently being dragged by the mouse
        /// </summary>
        public uiElement Dragging_Target { get; private set; } = null;
        public ePos Drag_Start { get; private set; } = null;
        /// <summary>
        /// Tracks the current location the mouse
        /// </summary>
        public ePos Location = new ePos();
        #endregion

        /// <summary>
        /// Starts a dragging operation for the given element
        /// </summary>
        /// <returns>Success</returns>
        public bool Start_Dragging(uiElement Sender, uiElement Targ, ItemDragEventArgs Args)
        {
            if (IsDragging) return false;
            Dragging_Target = Targ;
            Drag_Start = Args.Origin;
            Dragging_Target.Handle_DraggingStart(Sender, Args);
            if (Args.Abort) Stop_Dragging(Dragging_Target, Args);

            return true;
        }

        /// <summary>
        /// Stops an ongoing dragging operation,
        /// </summary>
        /// <param name="Sender">The element that is stopping the operation</param>
        /// <returns>Success</returns>
        public bool Stop_Dragging(uiElement Sender, ItemDragEventArgs Args)
        {
            if (!IsDragging) return false;

            if (Dragging_Target != null)
            {
                Drag_Start = null;
                if (!Args.Abort) Dragging_Target.Handle_DraggingConfirm(Sender, Args);
                Dragging_Target.Handle_DraggingEnd(Sender, Args);
            }

            Dragging_Target = null;
            Drag_Start = null;

            return true;
        }

    }
}
