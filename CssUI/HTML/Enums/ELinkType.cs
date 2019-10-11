using CssUI.Internal;

namespace CssUI.HTML
{
    [MetaEnum]
    public enum ELinkType : int
    {

        /// <summary>
        /// Gives alternate representations of the current document.
        /// </summary>
        [MetaKeyword("alternate")]
        Alternate,

        /// <summary>
        /// Gives the preferred URL for the current document.
        /// </summary>
        [MetaKeyword("canonical")]
        Canonical,

        /// <summary>
        /// Gives a link to the author of the current document or article.
        /// </summary>
        [MetaKeyword("author")]
        Author,

        /// <summary>
        /// Gives the permalink for the nearest ancestor section.
        /// </summary>
        [MetaKeyword("bookmark")]
        Bookmark,

        /// <summary>
        /// Specifies that the user agent should preemptively perform DNS resolution for the target resource's origin.
        /// </summary>
        [MetaKeyword("dns-prefetch")]
        DNS_Prefetch,

        /// <summary>
        /// Indicates that the referenced document is not part of the same site as the current document.
        /// </summary>
        [MetaKeyword("external")]
        External,

        /// <summary>
        /// Provides a link to context-sensitive help.
        /// </summary>
        [MetaKeyword("help")]
        Help,

        /// <summary>
        /// Imports an icon to represent the current document.
        /// </summary>
        [MetaKeyword("icon")]
        Icon,

        /// <summary>
        /// Specifies that the user agent must preemptively fetch the module script and store it in the document's module map for later evaluation. Optionally, the module's dependencies can be fetched as well.
        /// </summary>
        [MetaKeyword("modulepreload")]
        Modulepreload,

        /// <summary>
        /// Indicates that the main content of the current document is covered by the copyright license described by the referenced document.
        /// </summary>
        [MetaKeyword("license")]
        License,

        /// <summary>
        /// Indicates that the current document is a part of a series, and that the next document in the series is the referenced document.
        /// </summary>
        [MetaKeyword("next")]
        Next,

        /// <summary>
        /// Indicates that the current document's original author or publisher does not endorse the referenced document.
        /// </summary>
        [MetaKeyword("nofollow")]
        Nofollow,

        /// <summary>
        /// Creates a top-level browsing context that is not an auxiliary browsing context if the hyperlink would create either of those to begin with (i.e., has an appropriate target attribute value).
        /// </summary>
        [MetaKeyword("noopener")]
        Noopener,

        /// <summary>
        /// No `Referer` (sic) header will be included. Additionally, has the same effect as noopener.
        /// </summary>
        [MetaKeyword("noreferrer")]
        Noreferrer,

        /// <summary>
        /// Creates an auxiliary browsing context if the hyperlink would otherwise create a top-level browsing context that is not an auxiliary browsing context (i.e., has "_blank" as target attribute value).
        /// </summary>
        [MetaKeyword("opener")]
        Opener,

        /// <summary>
        /// Gives the address of the pingback server that handles pingbacks to the current document.
        /// </summary>
        [MetaKeyword("pingback")]
        Pingback,

        /// <summary>
        /// Specifies that the user agent should preemptively connect to the target resource's origin.
        /// </summary>
        [MetaKeyword("preconnect")]
        Preconnect,

        /// <summary>
        /// Specifies that the user agent should preemptively fetch and cache the target resource as it is likely to be required for a followup navigation.
        /// </summary>
        [MetaKeyword("prefetch")]
        Prefetch,

        /// <summary>
        /// Specifies that the user agent must preemptively fetch and cache the target resource for current navigation according to the potential destination given by the as attribute (and the priority associated with the corresponding destination).
        /// </summary>
        [MetaKeyword("preload")]
        Preload,

        /// <summary>
        /// Specifies that the user agent should preemptively fetch the target resource and process it in a way that helps deliver a faster response in the future.
        /// </summary>
        [MetaKeyword("prerender")]
        Prerender,

        /// <summary>
        /// Indicates that the current document is a part of a series, and that the previous document in the series is the referenced document.
        /// </summary>
        [MetaKeyword("prev")]
        Prev,

        /// <summary>
        /// Gives a link to a resource that can be used to search through the current document and its related pages.
        /// </summary>
        [MetaKeyword("search")]
        Search,

        /// <summary>
        /// Imports a style sheet.
        /// </summary>
        [MetaKeyword("stylesheet")]
        Stylesheet,

        /// <summary>
        /// Gives a tag (identified by the given address) that applies to the current document.
        /// </summary>
        [MetaKeyword("tag")]
        Tag,
    }
}
