using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// The area element represents either a hyperlink with some text and a corresponding area on an image map, or a dead area on an image map.
    /// </summary>
    [MetaElement("area")]
    public class HTMLAreaElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/image-maps.html#the-area-element */
        #region Properties
        #endregion

        #region Constructors
        public HTMLAreaElement(Document document) : base(document, "area")
        {
        }

        public HTMLAreaElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes  
        [CEReactions]
        public string alt
        {
            ...
        }

        [CEReactions] public string coords;
        [CEReactions] public string shape;
        [CEReactions] public string target;
        [CEReactions] public string download;
        [CEReactions] public string ping;
        [CEReactions] public string rel;
        readonly public DOMTokenList relList;
        [CEReactions] public string referrerPolicy;
        #endregion

        #region HyperlinkUtils Implementation
        /* Docs: https://html.spec.whatwg.org/multipage/links.html#htmlhyperlinkelementutils */

        [CEReactions] public string href;
        readonly public string origin;
        [CEReactions] public string protocol;
        [CEReactions] public string username;
        [CEReactions] public string password;
        [CEReactions] public string host;
        [CEReactions] public string hostname;
        [CEReactions] public string port;
        [CEReactions] public string pathname;
        [CEReactions] public string search;
        [CEReactions] public string hash;
        #endregion
    }
}
