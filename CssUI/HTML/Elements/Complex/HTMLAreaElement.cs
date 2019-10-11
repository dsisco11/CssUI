using CssUI.DOM;
using CssUI.DOM.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// The area element represents either a hyperlink with some text and a corresponding area on an image map, or a dead area on an image map.
    /// </summary>
    [MetaElement("area")]
    public class HTMLAreaElement : HyperlinkElement
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
            get => getAttribute(EAttributeName.Alt)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Alt, AttributeValue.From(value)));
        }

        [CEReactions] public IReadOnlyList<double> coords
        {
            get
            {
                string value = getAttribute(EAttributeName.Coords)?.AsString();
                if (ReferenceEquals(null, value) || value.Length <= 0) return Array.Empty<double>();

                var set = DOMCommon.Parse_Comma_Seperated_List(value.AsMemory());
                var numbers = new double[set.Count];
                for (int i=0; i<set.Count; i++)
                {
                    Serialization.HTMLParserCommon.Parse_FloatingPoint(set[i], out double outValue);
                    numbers[i] = outValue;
                }

                return numbers;
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => 
                {
                    var set = value.Select(o => o.ToString().AsMemory());
                    var serialized = DOMCommon.Serialize_Comma_Seperated_list(set);
                    setAttribute(EAttributeName.Coords, AttributeValue.From(serialized)); ;
                });
            }
        }

        [CEReactions] public EAreaShape shape
        {
            get => getAttribute(EAttributeName.Shape).AsEnum<EAreaShape>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Shape, AttributeValue.From(value)));
        }

        [CEReactions] public string target
        {
            get => getAttribute(EAttributeName.Target)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Target, AttributeValue.From(value)));
        }

        [CEReactions] public string download
        {
            get => getAttribute(EAttributeName.Download)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Download, AttributeValue.From(value)));
        }

        [CEReactions] public string ping
        {
            get => getAttribute(EAttributeName.Ping)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Ping, AttributeValue.From(value)));
        }

        [CEReactions] public string rel
        {
            get => getAttribute(EAttributeName.Rel)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Rel, AttributeValue.From(value)));
        }

        [CEReactions] public EReferrerPolicy referrerPolicy
        {
            get => getAttribute(EAttributeName.ReferrerPolicy).AsEnum<EReferrerPolicy>();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.ReferrerPolicy, AttributeValue.From(value)));
        }
        #endregion
    }
}
