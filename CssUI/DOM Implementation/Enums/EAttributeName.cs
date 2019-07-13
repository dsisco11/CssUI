using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// List of all common HTML attribute names
    /// </summary>
    [DomEnum]
    public enum EAttributeName : int
    {

        [DomKeyword("")]
        CUSTOM = -1,
        [DomKeyword("contenteditable")]
        ContentEditable,
        [DomKeyword("draggable")]
        Draggable,
    }
}
