using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EMimeType : int
{

    [EnumData("application/atom+xml")]
    Atom,

    [EnumData("application/octet-stream")]
    OctetStream,

    [EnumData("application/microdata+json")]
    JSON_Microdata,

    [EnumData("application/rss+xml")]
    RSS,

    /// <summary>
    /// </summary>
    [EnumData("application/x-www-form-urlencoded")]
    UrlEncoded,

    [EnumData("application/xhtml+xml")]
    XHTML,

    [EnumData("application/xml")]
    XmlApplication,

    /// <summary>
    /// </summary>
    [EnumData("multipart/form-data")]
    FormData,

    [EnumData("multipart/mixed")]
    Mixed,

    [EnumData("multipart/x-mixed-replace")]
    Streaming,

    [EnumData("text/cache-manifest")]
    Cache,

    [EnumData("text/css")]
    CSS,

    [EnumData("text/event-stream")]
    EventStream,

    [EnumData("text/javascript")]
    Javascript,

    [EnumData("text/json")]
    JSON,

    /// <summary>
    /// </summary>
    [EnumData("text/plain")]
    Plain,

    [EnumData("text/html")]
    HTML,

    [EnumData("text/ping")]
    Ping,

    [EnumData("text/uri-list")]
    UriList,

    [EnumData("text/vcard")]
    vCard,

    [EnumData("text/vtt")]
    WebVTT,

    [EnumData("text/xml")]
    XML,

    [EnumData("application/pdf")]
    PDF,

    [EnumData("application/postscript")]
    AdobePostscript,



    [EnumData("application/x-gzip")]
    GZIP,
    [EnumData("application/zip")]
    ZIP,
    [EnumData("application/x-rar-compressed")]
    RAR,


    [EnumData("audio/basic")]
    AudioBasic,
    [EnumData("audio/aiff")]
    AIFF,
    [EnumData("audio/mpeg")]
    MP3,
    [EnumData("audio/ogg")]
    OGG,
    [EnumData("audio/midi")]
    MIDI,
    [EnumData("audio/avi")]
    AVI,
    [EnumData("audio/wave")]
    WAVE,



    [EnumData("video/mp4")]
    MPEG4,
    [EnumData("video/webm")]
    WEBM,
    [EnumData("video/mpeg")]
    MPEG,



    [EnumData("image/x-icon")]
    XIcon,

    [EnumData("image/bmp")]
    BMP,

    [EnumData("image/gif")]
    GIF,

    [EnumData("image/webp")]
    WebP,

    [EnumData("image/jpeg")]
    JPEG,

    [EnumData("image/png")]
    PNG,

    [EnumData("image/svg+xml")]
    SVG,


}

