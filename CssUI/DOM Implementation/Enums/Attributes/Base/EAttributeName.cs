using CssUI.Internal;

namespace CssUI.DOM
{
    /// <summary>
    /// List of all common HTML attribute names
    /// </summary>
    [MetaEnum]
    public enum EAttributeName : int
    {

        /*[MetaKeyword("")]*/ // DO NOT GIVE NEGATIVE ENUM VALUES A KEYWORD
        CUSTOM = -1,
        [MetaKeyword("id")]
        ID = 0,
        [MetaKeyword("name")]
        Name,
        [MetaKeyword("class")]
        Class,
        [MetaKeyword("width")]
        Width,
        [MetaKeyword("height")]
        Height,

        [MetaKeyword("is")]
        IS,


        /* HTML Attributes */
        [MetaKeyword("title")]
        Title,
        [MetaKeyword("lang")]
        Lang,
        [MetaKeyword("dir")]
        Dir,
        [MetaKeyword("translate")]
        Translate,
        [MetaKeyword("nonce")]
        Nonce,
        [MetaKeyword("text")]
        Text,
        [MetaKeyword("label")]
        Label,


        [MetaKeyword("accesskey")]
        AccessKey,
        [MetaKeyword("spellcheck")]
        Spellcheck,
        [MetaKeyword("autocapitalize")]
        Autocapitalize,

        [MetaKeyword("contenteditable")]
        ContentEditable,
        [MetaKeyword("draggable")]
        Draggable,
        [MetaKeyword("tabindex")]
        TabIndex,
        [MetaKeyword("disabled")]
        Disabled,
        [MetaKeyword("hidden")]
        Hidden,
        [MetaKeyword("href")]
        HREF,
        [MetaKeyword("type")]
        Type,
        [MetaKeyword("slot")]
        Slot,
        [MetaKeyword("media")]
        Media,


        [MetaKeyword("dropzone")]
        Dropzone,


        [MetaKeyword("alt")]
        Alt,
        [MetaKeyword("src")]
        Src,
        [MetaKeyword("srcset")]
        SrcSet,
        [MetaKeyword("href")]
        Href,
        [MetaKeyword("sizes")]
        Sizes,
        [MetaKeyword("usemap")]
        UseMap,
        [MetaKeyword("ismap")]
        IsMap,
        [MetaKeyword("crossorigin")]
        CrossOrigin,
        [MetaKeyword("referrerpolicy")]
        ReferrerPolicy,
        [MetaKeyword("decoding")]
        Decoding,

        /* LABEL ATTRIBUTES */
        [MetaKeyword("for")]
        For,


        /* TABLE ATTRIBUTES */

        [MetaKeyword("span")]
        Span,
        [MetaKeyword("colspan")]
        ColSpan,
        [MetaKeyword("rowspan")]
        RowSpan,
        [MetaKeyword("cols")]
        Cols,
        [MetaKeyword("rows")]
        Rows,

        [MetaKeyword("headers")]
        Headers,

        [MetaKeyword("scope")]
        Scope,
        [MetaKeyword("abbr")]
        Abbr,

        /* INPUT */

        [MetaKeyword("checked")]
        Checked,
        [MetaKeyword("inputmode")]
        InputMode,
        [MetaKeyword("enterkeyhint")]
        EnterKeyHint,
        [MetaKeyword("autocomplete")]
        Autocomplete,
        [MetaKeyword("autofocus")]
        Autofocus,
        [MetaKeyword("multiple")]
        Multiple,
        [MetaKeyword("required")]
        Required,
        [MetaKeyword("size")]
        Size,

        [MetaKeyword("form")]
        Form,
        [MetaKeyword("value")]
        Value,
        [MetaKeyword("selected")]
        Selected,
        [MetaKeyword("dirname")]
        Dirname,


        /* FORMS */
    }
}
