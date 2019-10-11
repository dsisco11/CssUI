using CssUI.CSS.Enums;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    public class CSSPseudoElement : EventTarget
    {
        #region Properties
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
