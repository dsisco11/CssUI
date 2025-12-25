using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EMimeType : int
{

    [EnumRecordProperties("application/atom+xml")]
    Atom,

    [EnumRecordProperties("application/octet-stream")]
    OctetStream,

    [EnumRecordProperties("application/microdata+json")]
    JSON_Microdata,

    [EnumRecordProperties("application/rss+xml")]
    RSS,

    /// <summary>
    /// </summary>
    [EnumRecordProperties("application/x-www-form-urlencoded")]
    UrlEncoded,

    [EnumRecordProperties("application/xhtml+xml")]
    XHTML,

    [EnumRecordProperties("application/xml")]
    XmlApplication,

    /// <summary>
    /// </summary>
    [EnumRecordProperties("multipart/form-data")]
    FormData,

    [EnumRecordProperties("multipart/mixed")]
    Mixed,

    [EnumRecordProperties("multipart/x-mixed-replace")]
    Streaming,

    [EnumRecordProperties("text/cache-manifest")]
    Cache,

    [EnumRecordProperties("text/css")]
    CSS,

    [EnumRecordProperties("text/event-stream")]
    EventStream,

    [EnumRecordProperties("text/javascript")]
    Javascript,

    [EnumRecordProperties("text/json")]
    JSON,

    /// <summary>
    /// </summary>
    [EnumRecordProperties("text/plain")]
    Plain,

    [EnumRecordProperties("text/html")]
    HTML,

    [EnumRecordProperties("text/ping")]
    Ping,

    [EnumRecordProperties("text/uri-list")]
    UriList,

    [EnumRecordProperties("text/vcard")]
    vCard,

    [EnumRecordProperties("text/vtt")]
    WebVTT,

    [EnumRecordProperties("text/xml")]
    XML,

    [EnumRecordProperties("application/pdf")]
    PDF,

    [EnumRecordProperties("application/postscript")]
    AdobePostscript,



    [EnumRecordProperties("application/x-gzip")]
    GZIP,
    [EnumRecordProperties("application/zip")]
    ZIP,
    [EnumRecordProperties("application/x-rar-compressed")]
    RAR,


    [EnumRecordProperties("audio/basic")]
    AudioBasic,
    [EnumRecordProperties("audio/aiff")]
    AIFF,
    [EnumRecordProperties("audio/mpeg")]
    MP3,
    [EnumRecordProperties("audio/ogg")]
    OGG,
    [EnumRecordProperties("audio/midi")]
    MIDI,
    [EnumRecordProperties("audio/avi")]
    AVI,
    [EnumRecordProperties("audio/wave")]
    WAVE,



    [EnumRecordProperties("video/mp4")]
    MPEG4,
    [EnumRecordProperties("video/webm")]
    WEBM,
    [EnumRecordProperties("video/mpeg")]
    MPEG,



    [EnumRecordProperties("image/x-icon")]
    XIcon,

    [EnumRecordProperties("image/bmp")]
    BMP,

    [EnumRecordProperties("image/gif")]
    GIF,

    [EnumRecordProperties("image/webp")]
    WebP,

    [EnumRecordProperties("image/jpeg")]
    JPEG,

    [EnumRecordProperties("image/png")]
    PNG,

    [EnumRecordProperties("image/svg+xml")]
    SVG,


}

