using CssUI.Internal;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Lists all of the (currently) implemented HTML element-types
    /// </summary>
    [MetaEnum]
    public enum EElementTag : int
    {
        [MetaKeyword("div")]
        Div,
        [MetaKeyword("html")]
        Html,
        [MetaKeyword("body")]
        Body,
        [MetaKeyword("head")]
        Head,
        [MetaKeyword("template")]
        Template,
        [MetaKeyword("slot")]
        Slot,
    }
}
