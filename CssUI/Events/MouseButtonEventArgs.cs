using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class MouseButtonEventArgs : RoutedMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// The mouse button that triggered this event
        /// </summary>
        public readonly EMouseButton Button;
        /// <summary>
        /// Whether the culprit mouse button is pressed
        /// </summary>
        public readonly bool IsPressed;
        #endregion

        public MouseButtonEventArgs(object Source, int X, int Y, EMouseButton Button, bool IsPressed) : base(Source, X, Y)
        {
            this.Button = Button;
            this.IsPressed = IsPressed;
        }

        public MouseButtonEventArgs(object Source, PreviewMouseButtonEventArgs e) : base(Source, e)
        {
            this.Button = e.Button;
            this.IsPressed = e.IsPressed;
        }
    }

    public class PreviewMouseButtonEventArgs : PreviewMouseEventArgs
    {
        #region Properties
        /// <summary>
        /// The mouse button that triggered this event
        /// </summary>
        public readonly EMouseButton Button;
        /// <summary>
        /// Whether the culprit mouse button is pressed
        /// </summary>
        public readonly bool IsPressed;
        #endregion

        public PreviewMouseButtonEventArgs(PreviewMouseButtonEventArgs e) : base(e)
        {
            this.Button = e.Button;
            this.IsPressed = e.IsPressed;
        }

        public PreviewMouseButtonEventArgs(int X, int Y, EMouseButton Button, bool IsPressed) : base(X, Y)
        {
            this.Button = Button;
            this.IsPressed = IsPressed;
        }
    }
}
