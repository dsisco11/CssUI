using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The link element allows authors to link their document to other resources.
    /// </summary>
    /// <remarks>
    /// The link element represents metadata that can be used to link a document to other resources.
    /// It is commonly used to link to stylesheets.
    /// </remarks>
    [MetaElement("link")]
    public class HTMLLinkElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/semantics.html#the-link-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Metadata;
        #endregion

        #region Properties
        public readonly DOMEnumList<ELinkType> relList;
        #endregion

        #region Constructors
        public HTMLLinkElement(Document document) : this(document, "link")
        {
        }

        public HTMLLinkElement(Document document, string localName) : base(document, localName)
        {
            relList = new DOMEnumList<ELinkType>(this, EAttributeName.Rel);
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Address of the hyperlink
        /// </summary>
        [CEReactions]
        public string? href
        {
            get => getAttribute(EAttributeName.Href)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Href, AttributeValue.From(value)));
        }

        /// <summary>
        /// How the element handles crossorigin requests
        /// </summary>
        [CEReactions]
        public string? crossOrigin
        {
            get => getAttribute(EAttributeName.CrossOrigin)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.CrossOrigin, AttributeValue.From(value)));
        }

        /// <summary>
        /// Relationship between the document containing the hyperlink and the destination resource
        /// </summary>
        [CEReactions]
        public string? rel
        {
            get => getAttribute(EAttributeName.Rel)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Rel, AttributeValue.From(value)));
        }

        /// <summary>
        /// Applicable media
        /// </summary>
        [CEReactions]
        public string? media
        {
            get => getAttribute(EAttributeName.Media)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Media, AttributeValue.From(value)));
        }

        /// <summary>
        /// Integrity metadata used in Subresource Integrity checks
        /// </summary>
        [CEReactions]
        public string? integrity
        {
            get => getAttribute(EAttributeName.Integrity)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Integrity, AttributeValue.From(value)));
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
        /// Hint for the type of the referenced resource
        /// </summary>
        [CEReactions]
        public string? type
        {
            get => getAttribute(EAttributeName.Type)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Type, AttributeValue.From(value)));
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
        /// Whether the link is disabled
        /// </summary>
        [CEReactions]
        public bool disabled
        {
            get => hasAttribute(EAttributeName.Disabled);
            set
            {
                if (value)
                    CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Disabled, AttributeValue.From(string.Empty)));
                else
                    CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => removeAttribute(EAttributeName.Disabled));
            }
        }
        #endregion
    }
}
