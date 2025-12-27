using CssUI.CSS.Enums;
using CssUI.DOM.Events;

namespace CssUI.DOM;

public class CSSPseudoElement : EventTarget
{
    #region Properties
    public readonly EPseudoElement type;
    public readonly Element element;
    #endregion

    #region Constructor
    public CSSPseudoElement(EPseudoElement type, Element element)
    {
        this.type = type;
        this.element = element;
    }
    #endregion

}

