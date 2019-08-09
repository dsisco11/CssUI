using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Exceptions;
using System;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The area element represents either a hyperlink with some text and a corresponding area on an image map, or a dead area on an image map.
    /// </summary>
    [MetaElement("area")]
    public class HTMLAreaElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/image-maps.html#the-area-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing;
        #endregion

        #region Properties
        public readonly DOMEnumList<ELinkType> relList;
        #endregion

        #region Constructors
        public HTMLAreaElement(Document document) : this(document, "area")
        {
        }

        public HTMLAreaElement(Document document, string localName) : base(document, localName)
        {
            relList = new DOMEnumList<ELinkType>(this, EAttributeName.Rel);
        }
        #endregion

        #region Content Attributes  
        [CEReactions] public string alt
        {
            get => getAttribute(EAttributeName.Alt)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Alt, AttributeValue.From_String(value)));
        }

        [CEReactions] public IReadOnlyList<double> coords
        {
            get
            {
                string value = getAttribute(EAttributeName.Coords)?.Get_String();
                if (ReferenceEquals(null, value) || value.Length <= 0) return new double[0];

                var set = DOMCommon.Parse_Comma_Seperated_List(value.AsMemory());
                var numbers = new double[set.Count];
                for (int i=0; i<set.Count; i++)
                {
                    if (!Serialization.HTMLParserCommon.Parse_FloatingPoint(set[i], out double outValue))
                    {
                        throw new DomSyntaxError($"Cannot convert \"{set[i].ToString()}\" into number");
                    }

                    numbers[i] = outValue;
                }

                return numbers;
            }
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Coords, AttributeValue.From_String(value)));
        }

        [CEReactions] public EAreaShape shape
        {
            get => getAttribute(EAttributeName.Shape).Get_Enum<EAreaShape>();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Shape, AttributeValue.From_Enum(value)));
        }

        [CEReactions] public string target
        {
            get => getAttribute(EAttributeName.Target)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Target, AttributeValue.From_String(value)));
        }

        [CEReactions] public string download
        {
            get => getAttribute(EAttributeName.Download)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Download, AttributeValue.From_String(value)));
        }

        [CEReactions] public string ping
        {
            get => getAttribute(EAttributeName.Ping)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Ping, AttributeValue.From_String(value)));
        }

        [CEReactions] public string rel
        {
            get => getAttribute(EAttributeName.Rel)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Rel, AttributeValue.From_String(value)));
        }

        [CEReactions] public string referrerPolicy
        {
            get => getAttribute(EAttributeName.ReferrerPolicy)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.ReferrerPolicy, AttributeValue.From_String(value)));
        }
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
