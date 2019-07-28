using System;

namespace CssUI.DOM
{
    internal class TextSelection
    {
        #region Properties
        public int start = 0;
        public int end = 0;
        private ESelectionDirection _direction = ESelectionDirection.Forward;
        #endregion

        #region Accessors
        public ESelectionDirection direction
        {
            get => _direction;
            set
            {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#set-the-selection-direction */
                /* Windows has no None direction */
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    _direction = (value == ESelectionDirection.None) ? ESelectionDirection.Forward : value;
                }
                else
                {
                    _direction = value;
                }
            }
        }

        public bool HasSelection => ((start | end) == 0);
        #endregion

        #region Constructors
        public TextSelection()
        {
        }

        public TextSelection(TextSelection other)
        {
            start = other.start;
            end = other.end;
            direction = other.direction;
        }

        public TextSelection(int start, int end, ESelectionDirection direction = ESelectionDirection.Forward)
        {
            this.start = start;
            this.end = end;
            this.direction = direction;
        }
        #endregion

        /// <summary>
        /// Sets the start and end to both be 0
        /// </summary>
        public void Collapse()
        {
            start = end = 0;
        }

        public override bool Equals(object obj)
        {
            if ((obj is TextSelection other))
            {
                return (start == other.start) && (end == other.end) && (direction == other.direction);
            }

            return false;
        }

    }
}
