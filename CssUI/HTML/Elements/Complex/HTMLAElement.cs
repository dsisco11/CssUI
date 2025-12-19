using CssUI.DOM;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The a element represents a hyperlink.
    /// </summary>
    /// <remarks>
    /// If the a element has an href attribute, then it represents a hyperlink (a hypertext anchor) labeled by its contents.
    /// </remarks>
    [MetaElement("a")]
    public class HTMLAElement : HyperlinkElement
    {/* Docs: https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-a-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Properties
        public readonly DOMEnumList<ELinkType> relList;
        #endregion

        #region Constructors
        public HTMLAElement(Document document) : this(document, "a")
        {
        }

        public HTMLAElement(Document document, string localName) : base(document, localName)
        {
            relList = new DOMEnumList<ELinkType>(this, EAttributeName.Rel);
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Address of the hyperlink
        /// </summary>
        [CEReactions]
        public string? target
        {
            get => getAttribute(EAttributeName.Target)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Target, AttributeValue.From(value)));
        }

        /// <summary>
        /// Relationship between the location in the document containing the hyperlink and the destination resource
        /// </summary>
        [CEReactions]
        public string? rel
        {
            get => getAttribute(EAttributeName.Rel)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Rel, AttributeValue.From(value)));
        }

        /// <summary>
        /// Hint for the type of the referenced resource
        /// </summary>
        [CEReactions]
        public string? type
        {
            get => getAttribute(EAttributeName.Type)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Type, AttributeValue.From(value)));
        }

        /// <summary>
        /// Language of the linked resource
        /// </summary>
        [CEReactions]
        public string? hreflang
        {
            get => getAttribute(EAttributeName.HrefLang)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.HrefLang, AttributeValue.From(value)));
        }

        /// <summary>
        /// Whether to download the resource instead of navigating to it, and its file name if so
        /// </summary>
        [CEReactions]
        public string? download
        {
            get => getAttribute(EAttributeName.Download)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Download, AttributeValue.From(value)));
        }

        /// <summary>
        /// Referrer policy for fetches initiated by the element
        /// </summary>
        [CEReactions]
        public string? referrerPolicy
        {
            get => getAttribute(EAttributeName.ReferrerPolicy)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.ReferrerPolicy, AttributeValue.From(value)));
        }

        /// <summary>
        /// The text contents of the element
        /// </summary>
        public string text
        {
            get => textContent ?? string.Empty;
            set => textContent = value;
        }
        #endregion
    }
}
