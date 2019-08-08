﻿using CssUI.DOM;
using System;

namespace CssUI
{
    /// <summary>
    /// An element which can receive input focus and tracks a 'Selected' state
    /// </summary>
    public abstract class cssSelectableElement : cssCompoundElement, ISelectableElement
    {
        public static readonly new string CssTagName = "Selectable";

        #region Constructors
        public cssSelectableElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Flags_Add(EElementFlags.Focusable);
        }
        #endregion

        #region Selection Events
        public event Action<cssSelectableElement> onSelectedChanged;
        #endregion

        #region Selection State
        private bool selected = false;
        /// <summary>
        /// Selected status of the element
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    onSelectedChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Toggle the selection state
        /// </summary>
        public void Select()
        {
            Selected = !Selected;
        }
        #endregion
    }
}