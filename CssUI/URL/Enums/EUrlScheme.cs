using EnumRecords;

namespace CssUI.HTTP;

[EnumRecord<UrlSchemeProperties>]
public enum EUrlScheme : int
{/* Docs: https://url.spec.whatwg.org/#special-scheme */

    [EnumData("ftp", 21)]
    Ftp,

    [EnumData("file", -1)]
    File,

    [EnumData("blob", -1)]
    Blob,

    [EnumData("gopher", 70)]
    Gopher,

    [EnumData("http", 80)]
    Http,

    [EnumData("https", 443)]
    Https,

    [EnumData("ws", 80)]
    Ws,

    [EnumData("wss", 443)]
    Wss,
}

