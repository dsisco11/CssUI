using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Internal;
using System;

namespace CssUI.HTML
{
    public class HTMLObjectElement : FormAssociatedElement, IListedElement, ISubmittableElement, IBrowsingContextContainer
    {
        #region Backing Values
        #endregion

        #region Properties
        public BrowsingContext Nested_Browsing_Context { get; private set; }
        #endregion

        #region Constructors
        public HTMLObjectElement(Document document) : this(document, "object")
        {
        }

        public HTMLObjectElement(Document document, string localName) : base(document, localName)
        {
            // this.nestedBrowsingContext = 
        }
        #endregion

        #region Accessors
        public Window contentWindow => Nested_Browsing_Context?.WindowProxy;

        public Document contentDocument
        {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#concept-bcc-content-document */
            get
            {
                if (Nested_Browsing_Context == null) return null;
                var document = Nested_Browsing_Context.activeDocument;
                if (!StringCommon.StrEq(document.Origin.AsSpan(), nodeDocument.Origin.AsSpan())) return null;
                return document;
            }
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string data
        {
            get => getAttribute(EAttributeName.Data)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Data, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// A valid MIME-type
        /// </summary>
        [CEReactions]
        public override string type
        {
            get => getAttribute(EAttributeName.Type)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions]
        public string useMap
        {
            get => getAttribute(EAttributeName.UseMap)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.UseMap, AttributeValue.From_String(value)));
        }


        [CEReactions]
        public int width
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dimension-attributes */
            get => getAttribute(EAttributeName.Width).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Width, AttributeValue.From_Integer(value)));
        }
        [CEReactions]
        public int height
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dimension-attributes */
            get => getAttribute(EAttributeName.Height).Get_Int();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Height, AttributeValue.From_Integer(value)));
        }

        #endregion


        public Document getSVGDocument()
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dom-media-getsvgdocument */
            if (Nested_Browsing_Context == null)
                return null;

            if (!nodeDocument.Origin.AsSpan().Equals(Nested_Browsing_Context.activeDocument.Origin.AsSpan(), StringComparison.Ordinal))
                return null;

            /* XXX: finish the get SVG document stuff */
            throw new NotImplementedException();
        }

    }
}
