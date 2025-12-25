using EnumRecords;

namespace CssUI.HTTP;

[EnumRecord<UrlSchemeProperties>]
public enum EUrlScheme : int
{/* Docs: https://url.spec.whatwg.org/#special-scheme */

    [EnumRecordProperties("ftp", 21)]
    Ftp,

    [EnumRecordProperties("file", -1)]
    File,

    [EnumRecordProperties("blob", -1)]
    Blob,

    [EnumRecordProperties("gopher", 70)]
    Gopher,

    [EnumRecordProperties("http", 80)]
    Http,

    [EnumRecordProperties("https", 443)]
    Https,

    [EnumRecordProperties("ws", 80)]
    Ws,

    [EnumRecordProperties("wss", 443)]
    Wss,
}

