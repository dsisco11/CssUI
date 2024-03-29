﻿using CssUI.DOM;
using CssUI.DOM.Internal;
using System;

namespace CssUI.HTML
{
    /// <summary>
    /// The object element can represent an external resource, which, depending on the type of the resource, will either be treated as an image, as a nested browsing context, or as an external resource to be processed by a plugin.
    /// </summary>
    [MetaElement("object")]
    public sealed class HTMLObjectElement : FormAssociatedElement, IListedElement, ISubmittableElement, IBrowsingContextContainer
    {/* Docs: https://html.spec.whatwg.org/multipage/iframe-embed-object.html#the-object-element */

        #region Definition
        public override EContentCategories Categories
        {
            get
            {
                var flags = EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Embedded | EContentCategories.Palpable;
                if (hasAttribute(EAttributeName.UseMap))
                {
                    flags |= EContentCategories.Interactive;
                }

                return flags;
            }
        }
        #endregion

        #region Properties
        public BrowsingContext Nested_Browsing_Context { get; private set; }
        internal EmbeddedContent Content { get; private set; }

#if ENABLE_OBJECT_PLUGINS
        public object Plugin { get; set; }
#endif
        #endregion

        #region Browsing Context Container
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
                if (!document.document_origin.IsSameOriginDomain(nodeDocument.document_origin)) return null;
                return document;
            }
        }

        internal override int? Get_Intrinsic_Width()
        {/* XXX: return the replaced contents intrinsic width/height or NULL */
            if (Content != null)
            {
                return Content.Intrinsic_Width;
            }

            return null;
        }
        internal override int? Get_Intrinsic_Height()
        {/* XXX: return the replaced contents intrinsic width/height or NULL */
            if (Content != null)
            {
                return Content.Intrinsic_Height;
            }

            return null;
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public string data
        {
            get => getAttribute(EAttributeName.Data)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Data, AttributeValue.From(value)));
        }

        /// <summary>
        /// A valid MIME-type
        /// </summary>
        [CEReactions]
        public override string type
        {
            get => getAttribute(EAttributeName.Type)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Type, AttributeValue.From(value)));
        }

        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }

        [CEReactions]
        public string useMap
        {
            get => getAttribute(EAttributeName.UseMap)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.UseMap, AttributeValue.From(value)));
        }



        [CEReactions]
        public int? width
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dimension-attributes */
            get
            {
                return getAttribute(EAttributeName.Width)?.AsInt();
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Width, !value.HasValue ? null : AttributeValue.From(value.Value)));
            }
        }
        [CEReactions]
        public int? height
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dimension-attributes */
            get
            {
                return getAttribute(EAttributeName.Height)?.AsInt();
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Height, !value.HasValue ? null : AttributeValue.From(value.Value)));
            }
        }

        #endregion


        public Document getSVGDocument()
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dom-media-getsvgdocument */
            if (Nested_Browsing_Context == null)
                return null;

            if (!nodeDocument.document_origin.IsSameOriginDomain(Nested_Browsing_Context.activeDocument.document_origin))
                return null;

            /* XXX: Finish the SVG document stuff */
            throw new NotImplementedException();
        }

    }
}
