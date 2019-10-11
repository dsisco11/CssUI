using CssUI.Internal;

namespace CssUI.HTTP
{
    [MetaEnum]
    public enum EUrlScheme : int
    {/* Docs: https://url.spec.whatwg.org/#special-scheme */

        [MetaKeyword("ftp", 21)]
        Ftp,

        [MetaKeyword("file")]
        File,

        [MetaKeyword("blob")]
        Blob,

        [MetaKeyword("gopher", 70)]
        Gopher,

        [MetaKeyword("http", 80)]
        Http,

        [MetaKeyword("https", 443)]
        Https,

        [MetaKeyword("ws", 80)]
        Ws,

        [MetaKeyword("wss", 443)]
        Wss,
    }
}
