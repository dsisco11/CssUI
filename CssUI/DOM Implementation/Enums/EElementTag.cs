using CssUI.DOM.Internal;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Lists all of the (currently) implemented HTML element-types
    /// </summary>
    [DomEnum]
    public enum EElementTag : int
    {
        [DomKeyword("div")]
        Div,
        [DomKeyword("html")]
        Html,
        [DomKeyword("body")]
        Body,
        [DomKeyword("head")]
        Head,
        [DomKeyword("template")]
        Template,
        [DomKeyword("slot")]
        Slot,
    }
}
