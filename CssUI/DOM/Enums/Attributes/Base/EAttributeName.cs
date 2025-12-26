using EnumRecords;

namespace CssUI.DOM;

/// <summary>
/// List of all common HTML attribute names
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EAttributeName : int
{

    /*[EnumData("")]*/ // DO NOT GIVE NEGATIVE ENUM VALUES A KEYWORD
    [Ignore]
    CUSTOM = -1,
    [EnumData("id")]
    ID = 0,
    [EnumData("name")]
    Name,
    [EnumData("class")]
    Class,
    [EnumData("width")]
    Width,
    [EnumData("height")]
    Height,

    [EnumData("is")]
    IS,


    /* HTML Attributes */
    [EnumData("title")]
    Title,
    [EnumData("lang")]
    Lang,
    [EnumData("dir")]
    Dir,
    [EnumData("translate")]
    Translate,
    [EnumData("nonce")]
    Nonce,
    [EnumData("text")]
    Text,
    [EnumData("label")]
    Label,
    /// <summary>
    /// Every HTML element may have an itemprop attribute specified, if doing so adds one or more properties to one or more items (as defined below).
    /// <para>ReadMore: https://html.spec.whatwg.org/multipage/microdata.html#names:-the-itemprop-attribute</para>
    /// </summary>
    [EnumData("itemprop")]
    ItemProp,


    [EnumData("accesskey")]
    AccessKey,
    [EnumData("spellcheck")]
    Spellcheck,
    [EnumData("autocapitalize")]
    Autocapitalize,

    [EnumData("contenteditable")]
    ContentEditable,
    [EnumData("draggable")]
    Draggable,
    [EnumData("tabindex")]
    TabIndex,
    [EnumData("disabled")]
    Disabled,
    [EnumData("hidden")]
    Hidden,
    [EnumData("type")]
    Type,
    [EnumData("slot")]
    Slot,
    [EnumData("media")]
    Media,


    [EnumData("dropzone")]
    Dropzone,


    [EnumData("alt")]
    Alt,
    [EnumData("src")]
    Src,
    [EnumData("srcset")]
    SrcSet,
    [EnumData("href")]
    Href,
    [EnumData("hreflang")]
    HrefLang,
    [EnumData("sizes")]
    Sizes,
    [EnumData("usemap")]
    UseMap,
    [EnumData("ismap")]
    IsMap,
    [EnumData("crossorigin")]
    CrossOrigin,
    [EnumData("referrerpolicy")]
    ReferrerPolicy,
    [EnumData("decoding")]
    Decoding,

    [EnumData("enctype")]
    EncType,
    [EnumData("method")]
    Method,
    [EnumData("novalidate")]
    NoValidate,
    [EnumData("target")]
    Target,

    /* LINK ATTRIBUTES */
    [EnumData("rel")]
    Rel,

    /* QUOTE ATTRIBUTES */
    [EnumData("cite")]
    Cite,

    /* LABEL ATTRIBUTES */
    [EnumData("for")]
    For,

    /* DIALOG ATTRIBUTES */
    [EnumData("open")]
    Open,


    /* TABLE ATTRIBUTES */
    [EnumData("span")]
    Span,
    [EnumData("colspan")]
    ColSpan,
    [EnumData("rowspan")]
    RowSpan,
    [EnumData("cols")]
    Cols,
    [EnumData("rows")]
    Rows,

    [EnumData("headers")]
    Headers,

    [EnumData("scope")]
    Scope,
    [EnumData("abbr")]
    Abbr,

    /* INPUT */

    [EnumData("checked")]
    Checked,
    [EnumData("inputmode")]
    InputMode,
    [EnumData("enterkeyhint")]
    EnterKeyHint,
    [EnumData("autocomplete")]
    Autocomplete,
    [EnumData("autofocus")]
    Autofocus,
    [EnumData("multiple")]
    Multiple,
    [EnumData("required")]
    Required,
    [EnumData("size")]
    Size,
    [EnumData("placeholder")]
    Placeholder,

    [EnumData("accept")]
    Accept,

    [EnumData("action")]
    Action,
    [EnumData("formaction")]
    FormAction,
    [EnumData("formenctype")]
    FormEncType,
    [EnumData("formmethod")]
    FormMethod,
    [EnumData("formnovalidate")]
    FormNoValidate,
    [EnumData("formtarget")]
    FormTarget,
    [EnumData("list")]
    List,
    [EnumData("min")]
    Min,
    [EnumData("max")]
    Max,
    [EnumData("pattern")]
    Pattern,
    [EnumData("step")]
    Step,

    /* TEXTAREA */
    [EnumData("readonly")]
    ReadOnly,
    [EnumData("wrap")]
    Wrap,

    [EnumData("form")]
    Form,
    [EnumData("value")]
    Value,
    [EnumData("selected")]
    Selected,
    [EnumData("dirname")]
    Dirname,
    [EnumData("minlength")]
    MinLength,
    [EnumData("maxlength")]
    MaxLength,


    /* FORMS */
    [EnumData("accept-charset")]
    AcceptCharset,


    /* OBJECT */
    [EnumData("data")]
    Data,

    /* IFRAME */
    [EnumData("sandbox")]
    Sandbox,
    [EnumData("srcdoc")]
    Srcdoc,
    [EnumData("allow")]
    Allow,

    /* AREA */
    [EnumData("coords")]
    Coords,
    [EnumData("shape")]
    Shape,
    [EnumData("ping")]
    Ping,
    [EnumData("download")]
    Download,

    /* SCRIPT/LINK INTEGRITY */
    [EnumData("integrity")]
    Integrity,
}

