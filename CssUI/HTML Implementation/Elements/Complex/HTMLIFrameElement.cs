using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Internal;
using System;

namespace CssUI.HTML
{
    /// <summary>
    /// IFrame elements are not fully supported
    /// </summary>
    [MetaElement("iframe")]
    public class HTMLIFrameElement : HTMLElement, IBrowsingContextContainer
    {/* Docs: https://html.spec.whatwg.org/multipage/iframe-embed-object.html#the-iframe-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Embedded | EContentCategories.Interactive | EContentCategories.Palpable;
        #endregion

        #region Properties
        public readonly DOMTokenList sandbox;
        #endregion

        #region Browsing Context Container
        public BrowsingContext Nested_Browsing_Context { get; private set; }
        #endregion

        #region Constructors
        public HTMLIFrameElement(Document document) : this(document, "iframe")
        {
        }

        public HTMLIFrameElement(Document document, string localName) : base(document, localName)
        {
            sandbox = new DOMTokenList(this, EAttributeName.Sandbox);
        }
        #endregion

        #region Accessors
        public Document contentDocument
        {
            get
            {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#concept-bcc-content-document */
                if (Nested_Browsing_Context == null) return null;
                var context = Nested_Browsing_Context;
                var document = context.activeDocument;
                var topContext = ownerDocument.BrowsingContext.Get_Top_Level_Browsing_Context();

                if (!DOMCommon.Is_Same_Origin_Domain(document.Origin, topContext.activeDocument.Origin)) return null;
                return document;
            }
        }

        public Window contentWindow
        {/* Docs: https://html.spec.whatwg.org/multipage/iframe-embed-object.html#dom-iframe-contentwindow */
            get => Nested_Browsing_Context?.WindowProxy;
        }
        #endregion

        #region Content publics
        [CEReactions] public string src
        {
            get => getAttribute(EAttributeName.Src).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Src, AttributeValue.From_String(value)));
        }

        [CEReactions] public string srcdoc;

        [CEReactions] public string name
        {
            get => getAttribute(EAttributeName.Name).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions] public string allow
        {
            get => getAttribute(EAttributeName.Allow).Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Allow, AttributeValue.From_String(value)));
        }

        [CEReactions] public bool allowFullscreen;
        [CEReactions] public bool allowPaymentRequest;
        [CEReactions] public string width;
        [CEReactions] public string height;
        [CEReactions] public string referrerPolicy;
        #endregion

        Document getSVGDocument()
        {/* Docs: https://html.spec.whatwg.org/multipage/embedded-content-other.html#dom-media-getsvgdocument */
            /* XXX: SVG */
            throw new NotImplementedException();
        }


    }
}
