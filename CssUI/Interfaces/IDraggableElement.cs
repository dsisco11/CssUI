using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Interface for an element that can be moved around via the user 'dragging' it with the mouse
    /// </summary>
    public interface IDraggableElement
    {
        /// <summary>
        /// Used to draw any applicable effect shown during the dragging process for this element
        /// </summary>
        void Draw_DraggingEffect();

        /// <summary>
        /// Fires at the start of the dragging process
        /// </summary>
        void Handle_DraggingStart(uiElement Sender, DomItemDragEventArgs Args);
        /// <summary>
        /// Fired as the element is being dragged
        /// </summary>
        void Handle_DraggingUpdate(uiElement Sender, DomItemDragEventArgs Args);
        /// <summary>
        /// Fires when the dragging operation is confirmed (ended without cancelling)
        /// </summary>
        void Handle_DraggingConfirm(uiElement Sender, DomItemDragEventArgs Args);
        /// <summary>
        /// Fires when the dragging operation ends (including being cancelled)
        /// Called AFTER <see cref="Handle_DraggingConfirm"/>
        /// </summary>
        void Handle_DraggingEnd(uiElement Sender, DomItemDragEventArgs Args);
    }
}
