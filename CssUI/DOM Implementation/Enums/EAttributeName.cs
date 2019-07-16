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
        [DomKeyword("id")]
        ID = 0,
        [DomKeyword("name")]
        Name,
        [DomKeyword("class")]
        Class,
        [DomKeyword("contenteditable")]
        ContentEditable,
        [DomKeyword("draggable")]
        Draggable,
        [DomKeyword("tabindex")]
        TabIndex,
        [DomKeyword("disabled")]
        Disabled,
        [DomKeyword("hidden")]
        Hidden,
        [DomKeyword("href")]
        HREF,
        [DomKeyword("type")]
        Type,
    }
}
