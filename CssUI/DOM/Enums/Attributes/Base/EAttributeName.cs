using EnumRecords;

namespace CssUI.DOM;

/// <summary>
/// List of all common HTML attribute names
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EAttributeName : int
{

    /*[EnumRecordProperties("")]*/ // DO NOT GIVE NEGATIVE ENUM VALUES A KEYWORD
    CUSTOM = -1,
    [EnumRecordProperties("id")]
    ID = 0,
    [EnumRecordProperties("name")]
    Name,
    [EnumRecordProperties("class")]
    Class,
    [EnumRecordProperties("width")]
    Width,
    [EnumRecordProperties("height")]
    Height,

    [EnumRecordProperties("is")]
    IS,


    /* HTML Attributes */
    [EnumRecordProperties("title")]
    Title,
    [EnumRecordProperties("lang")]
    Lang,
    [EnumRecordProperties("dir")]
    Dir,
    [EnumRecordProperties("translate")]
    Translate,
    [EnumRecordProperties("nonce")]
    Nonce,
    [EnumRecordProperties("text")]
    Text,
    [EnumRecordProperties("label")]
    Label,
    /// <summary>
    /// Every HTML element may have an itemprop attribute specified, if doing so adds one or more properties to one or more items (as defined below).
    /// <para>ReadMore: https://html.spec.whatwg.org/multipage/microdata.html#names:-the-itemprop-attribute</para>
    /// </summary>
    [EnumRecordProperties("itemprop")]
    ItemProp,


    [EnumRecordProperties("accesskey")]
    AccessKey,
    [EnumRecordProperties("spellcheck")]
    Spellcheck,
    [EnumRecordProperties("autocapitalize")]
    Autocapitalize,

    [EnumRecordProperties("contenteditable")]
    ContentEditable,
    [EnumRecordProperties("draggable")]
    Draggable,
    [EnumRecordProperties("tabindex")]
    TabIndex,
    [EnumRecordProperties("disabled")]
    Disabled,
    [EnumRecordProperties("hidden")]
    Hidden,
    [EnumRecordProperties("type")]
    Type,
    [EnumRecordProperties("slot")]
    Slot,
    [EnumRecordProperties("media")]
    Media,


    [EnumRecordProperties("dropzone")]
    Dropzone,


    [EnumRecordProperties("alt")]
    Alt,
    [EnumRecordProperties("src")]
    Src,
    [EnumRecordProperties("srcset")]
    SrcSet,
    [EnumRecordProperties("href")]
    Href,
    [EnumRecordProperties("hreflang")]
    HrefLang,
    [EnumRecordProperties("sizes")]
    Sizes,
    [EnumRecordProperties("usemap")]
    UseMap,
    [EnumRecordProperties("ismap")]
    IsMap,
    [EnumRecordProperties("crossorigin")]
    CrossOrigin,
    [EnumRecordProperties("referrerpolicy")]
    ReferrerPolicy,
    [EnumRecordProperties("decoding")]
    Decoding,

    [EnumRecordProperties("enctype")]
    EncType,
    [EnumRecordProperties("method")]
    Method,
    [EnumRecordProperties("novalidate")]
    NoValidate,
    [EnumRecordProperties("target")]
    Target,

    /* LINK ATTRIBUTES */
    [EnumRecordProperties("rel")]
    Rel,

    /* QUOTE ATTRIBUTES */
    [EnumRecordProperties("cite")]
    Cite,

    /* LABEL ATTRIBUTES */
    [EnumRecordProperties("for")]
    For,

    /* DIALOG ATTRIBUTES */
    [EnumRecordProperties("open")]
    Open,


    /* TABLE ATTRIBUTES */
    [EnumRecordProperties("span")]
    Span,
    [EnumRecordProperties("colspan")]
    ColSpan,
    [EnumRecordProperties("rowspan")]
    RowSpan,
    [EnumRecordProperties("cols")]
    Cols,
    [EnumRecordProperties("rows")]
    Rows,

    [EnumRecordProperties("headers")]
    Headers,

    [EnumRecordProperties("scope")]
    Scope,
    [EnumRecordProperties("abbr")]
    Abbr,

    /* INPUT */

    [EnumRecordProperties("checked")]
    Checked,
    [EnumRecordProperties("inputmode")]
    InputMode,
    [EnumRecordProperties("enterkeyhint")]
    EnterKeyHint,
    [EnumRecordProperties("autocomplete")]
    Autocomplete,
    [EnumRecordProperties("autofocus")]
    Autofocus,
    [EnumRecordProperties("multiple")]
    Multiple,
    [EnumRecordProperties("required")]
    Required,
    [EnumRecordProperties("size")]
    Size,
    [EnumRecordProperties("placeholder")]
    Placeholder,

    [EnumRecordProperties("accept")]
    Accept,

    [EnumRecordProperties("action")]
    Action,
    [EnumRecordProperties("formaction")]
    FormAction,
    [EnumRecordProperties("formenctype")]
    FormEncType,
    [EnumRecordProperties("formmethod")]
    FormMethod,
    [EnumRecordProperties("formnovalidate")]
    FormNoValidate,
    [EnumRecordProperties("formtarget")]
    FormTarget,
    [EnumRecordProperties("list")]
    List,
    [EnumRecordProperties("min")]
    Min,
    [EnumRecordProperties("max")]
    Max,
    [EnumRecordProperties("pattern")]
    Pattern,
    [EnumRecordProperties("step")]
    Step,

    /* TEXTAREA */
    [EnumRecordProperties("readonly")]
    ReadOnly,
    [EnumRecordProperties("wrap")]
    Wrap,

    [EnumRecordProperties("form")]
    Form,
    [EnumRecordProperties("value")]
    Value,
    [EnumRecordProperties("selected")]
    Selected,
    [EnumRecordProperties("dirname")]
    Dirname,
    [EnumRecordProperties("minlength")]
    MinLength,
    [EnumRecordProperties("maxlength")]
    MaxLength,


    /* FORMS */
    [EnumRecordProperties("accept-charset")]
    AcceptCharset,


    /* OBJECT */
    [EnumRecordProperties("data")]
    Data,

    /* IFRAME */
    [EnumRecordProperties("sandbox")]
    Sandbox,
    [EnumRecordProperties("srcdoc")]
    Srcdoc,
    [EnumRecordProperties("allow")]
    Allow,

    /* AREA */
    [EnumRecordProperties("coords")]
    Coords,
    [EnumRecordProperties("shape")]
    Shape,
    [EnumRecordProperties("ping")]
    Ping,
    [EnumRecordProperties("download")]
    Download,

    /* SCRIPT/LINK INTEGRITY */
    [EnumRecordProperties("integrity")]
    Integrity,
}

