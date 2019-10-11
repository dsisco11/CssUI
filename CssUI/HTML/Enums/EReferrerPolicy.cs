using CssUI.Internal;

namespace CssUI.HTML
{
    [MetaEnum]
    public enum EReferrerPolicy : int
    {
        /// <summary>
        /// </summary>
        [MetaKeyword("")]
        None,

        /// <summary>
        /// The simplest policy is "no-referrer", which specifies that no referrer information is to be sent along with requests made from a particular request client to any origin. The header will be omitted entirely.
        /// </summary>
        [MetaKeyword("no-referrer")]
        No_Referrer,

        /// <summary>
        /// The "no-referrer-when-downgrade" policy sends a full URL along with requests from a TLS-protected environment settings object to a potentially trustworthy URL, and requests from clients which are not TLS-protected to any origin.
        /// </summary>
        [MetaKeyword("no-referrer-when-downgrade")]
        No_Referrer_When_Downgrade,

        /// <summary>
        /// The "same-origin" policy specifies that a full URL, stripped for use as a referrer, is sent as referrer information when making same-origin requests from a particular client.
        /// </summary>
        [MetaKeyword("same-origin")]
        Same_Origin,

        /// <summary>
        /// The "origin" policy specifies that only the ASCII serialization of the origin of the request client is sent as referrer information when making both same-origin requests and cross-origin requests from a particular client.
        /// </summary>
        [MetaKeyword("origin")]
        Origin,

        /// <summary>
        /// The "strict-origin" policy sends the ASCII serialization of the origin of the request client when making requests:
        /// </summary>
        [MetaKeyword("strict-origin")]
        Strict_Origin,

        /// <summary>
        /// The "origin-when-cross-origin" policy specifies that a full URL, stripped for use as a referrer, is sent as referrer information when making same-origin requests from a particular request client, and only the ASCII serialization of the origin of the request client is sent as referrer information when making cross-origin requests from a particular client.
        /// </summary>
        [MetaKeyword("origin-when-cross-origin")]
        Origin_When_Cross_Origin,

        /// <summary>
        /// The "strict-origin-when-cross-origin" policy specifies that a full URL, stripped for use as a referrer, is sent as referrer information when making same-origin requests from a particular request client, and only the ASCII serialization of the origin of the request client when making cross-origin requests:
        /// <para>from a TLS-protected environment settings object to a potentially trustworthy URL, and</para>
        /// <para>from non-TLS-protected environment settings objects to any origin.</para>
        /// <para>Requests from TLS-protected clients to non- potentially trustworthy URLs, on the other hand, will contain no referrer information. A Referer HTTP header will not be sent.</para>
        /// </summary>
        [MetaKeyword("strict-origin-when-cross-origin")]
        Strict_Origin_When_Cross_Origin,

        /// <summary>
        /// The "unsafe-url" policy specifies that a full URL, stripped for use as a referrer, is sent along with both cross-origin requests and same-origin requests made from a particular client.
        /// </summary>
        [MetaKeyword("unsafe-url")]
        Unsafe_Url,
    }
}
