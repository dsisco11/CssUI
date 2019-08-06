using System;

namespace CssUI.DOM
{
    [Flags]
    public enum EMimeGroup : int
    {
        /// <summary>
        /// An image MIME type is a MIME type whose type is "image".
        /// </summary>
        Image = (1 << 1),


        /// <summary>
        /// An audio MIME type is any MIME type whose type is "audio" or whose essence is "application/ogg".
        /// </summary>
        Audio = (1 << 2),


        /// <summary>
        /// An video MIME type is any MIME type whose type is "video"
        /// </summary>
        Video = (1 << 3),


        /// <summary>
        /// A font MIME type is any MIME type whose type is "font", or whose essence is one of the following: [RFC8081]
        /// <para>
        /// application/font-cff,
        /// application/font-off,
        /// application/font-sfnt,
        /// application/font-ttf,
        /// application/font-woff,
        /// application/vnd.ms-fontobject,
        /// application/vnd.ms-opentype,
        /// </para>
        /// </summary>
        Font = (1 << 4),


        /// <summary>
        /// A ZIP-based MIME type is any MIME type whose subtype ends in "+zip" or whose essence is one of the following:
        /// <para>
        /// application/zip
        /// </para>
        /// </summary>
        Zip = (1 << 5),


        /// <summary>
        /// An archive MIME type is any MIME type whose essence is one of the following:
        /// <para>
        /// application/x-rar-compressed,
        /// application/zip,
        /// application/x-gzip,
        /// </para>
        /// </summary>
        Archive = (1 << 6),


        /// <summary>
        /// An XML MIME type is any MIME type whose subtype ends in "+xml" or whose essence is "text/xml" or "application/xml". [RFC7303]
        /// </summary>
        XML = (1 << 7),


        /// <summary>
        /// An HTML MIME type is any MIME type whose essence is "text/html".
        /// </summary>
        HTML = (1 << 8),


        /// <summary>
        /// A scriptable MIME type is an XML MIME type, HTML MIME type, or any MIME type whose essence is "application/pdf".
        /// </summary>
        Scriptable = (1 << 9),


        /// <summary>
        /// A JavaScript MIME type is any MIME type whose essence is one of the following:
        /// <para>
        /// application/ecmascript,
        /// application/javascript,
        /// application/x-ecmascript,
        /// application/x-javascript,
        /// text/ecmascript,
        /// text/javascript,
        /// text/javascript1.0,
        /// text/javascript1.1,
        /// text/javascript1.2,
        /// text/javascript1.3,
        /// text/javascript1.4,
        /// text/javascript1.5,
        /// text/jscript,
        /// text/livescript,
        /// text/x-ecmascript,
        /// text/x-javascript,
        /// </para>
        /// </summary>
        Javascript = (1 << 10),


        /// <summary>
        /// A JSON MIME type is any MIME type whose subtype ends in "+json" or whose essence is "application/json" or "text/json".
        /// </summary>
        JSON = (1 << 11),
    }
}
