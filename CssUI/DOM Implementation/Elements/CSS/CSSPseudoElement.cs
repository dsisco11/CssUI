using CssUI.CSS.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;
using CssUI.DOM.Interfaces;
using CssUI.DOM.Internal;
using System.Collections;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class CSSPseudoElement : EventTarget
    {
        #region Properties
        /* XXX: Replace with an AtomicName (need an enum) */
        public readonly AtomicName<EPseudoElement> type;
        public readonly Element element;
        #endregion

        #region Constructor
        public CSSPseudoElement(AtomicName<EPseudoElement> type, Element element)
        {
            this.type = type;
            this.element = element;
        }
        #endregion

    }
}
