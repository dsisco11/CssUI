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
        [DomKeyword("width")]
        Width,
        [DomKeyword("height")]
        Height,

        /* HTML Attributes */
        [DomKeyword("title")]
        Title,
        [DomKeyword("lang")]
        Lang,
        [DomKeyword("dir")]
        Dir,
        [DomKeyword("translate")]
        Translate,
        [DomKeyword("nonce")]
        Nonce,


        [DomKeyword("accesskey")]
        AccessKey,
        [DomKeyword("spellcheck")]
        Spellcheck,
        [DomKeyword("autocapitalize")]
        AutoCapitalize,

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
        [DomKeyword("slot")]
        Slot,
        [DomKeyword("media")]
        Media,

        [DomKeyword("alt")]
        Alt,
        [DomKeyword("src")]
        Src,
        [DomKeyword("srcset")]
        SrcSet,
        [DomKeyword("href")]
        Href,
        [DomKeyword("sizes")]
        Sizes,
        [DomKeyword("usemap")]
        UseMap,
        [DomKeyword("ismap")]
        IsMap,
        [DomKeyword("crossorigin")]
        CrossOrigin,
        [DomKeyword("referrerpolicy")]
        ReferrerPolicy,
        [DomKeyword("decoding")]
        Decoding,

        /* TABLE ATTRIBUTES */

        [DomKeyword("span")]
        Span,
        [DomKeyword("colspan")]
        ColSpan,
        [DomKeyword("rowspan")]
        RowSpan,

        [DomKeyword("headers")]
        Headers,

        [DomKeyword("scope")]
        Scope,
        [DomKeyword("abbr")]
        Abbr,

        /* INPUT */

        [DomKeyword("inputmode")]
        InputMode,
        [DomKeyword("enterkeyhint")]
        EnterKeyHint,
    }
}
