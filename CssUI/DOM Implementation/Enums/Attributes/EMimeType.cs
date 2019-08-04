using CssUI.Internal;

namespace CssUI.DOM
{
    [MetaEnum]
    public enum EMimeType : int
    {

        [MetaKeyword("application/atom+xml")]
        Atom,

        [MetaKeyword("application/octet-stream")]
        OctetStream,

        [MetaKeyword("application/microdata+json")]
        JSON_Microdata,

        [MetaKeyword("application/rss+xml")]
        RSS,

        /// <summary>
        /// </summary>
        [MetaKeyword("application/x-www-form-urlencoded")]
        UrlEncoded,

        [MetaKeyword("application/xhtml+xml")]
        XHTML,

        [MetaKeyword("application/xml")]
        XmlApplication,

        [MetaKeyword("image/gif")]
        GIF,

        [MetaKeyword("image/jpeg")]
        JPEG,

        [MetaKeyword("image/png")]
        PNG,

        [MetaKeyword("image/svg+xml")]
        SVG,

        /// <summary>
        /// </summary>
        [MetaKeyword("multipart/form-data")]
        FormData,

        [MetaKeyword("multipart/mixed")]
        Mixed,

        [MetaKeyword("multipart/x-mixed-replace")]
        Streaming,

        [MetaKeyword("text/cache-manifest")]
        Cache,

        [MetaKeyword("text/css")]
        CSS,

        [MetaKeyword("text/event-stream")]
        EventStream,

        [MetaKeyword("text/javascript")]
        Javascript,

        [MetaKeyword("text/json")]
        JSON,

        /// <summary>
        /// </summary>
        [MetaKeyword("text/plain")]
        Plain,

        [MetaKeyword("text/html")]
        HTML,

        [MetaKeyword("text/ping")]
        Ping,

        [MetaKeyword("text/uri-list")]
        UriList,

        [MetaKeyword("text/vcard")]
        vCard,

        [MetaKeyword("text/vtt")]
        WebVTT,

        [MetaKeyword("text/xml")]
        XML,

        [MetaKeyword("video/mp4")]
        MPEG4,

        [MetaKeyword("video/mpeg")]
        MPEG,
    }
}
