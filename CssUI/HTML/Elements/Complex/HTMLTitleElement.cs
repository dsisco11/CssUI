using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Linq;

namespace CssUI.HTML
{
    /// <summary>
    /// The title element represents the document's title or name. Authors should use titles that identify their documents even when they are used out of context, for example in a user's history or bookmarks, or in search results. The document's title is often different from its first heading, since the first heading does not have to stand alone when taken out of context.
    /// </summary>
    [MetaElement("title")]
    public sealed class HTMLTitleElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/semantics.html#the-title-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.MetaData;
        #endregion

        #region Constructors
        public HTMLTitleElement(Document document) : base(document, "title")
        {
        }

        public HTMLTitleElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        [CEReactions] public string Text
        {
            get
            {
                var textNodes = DOMCommon.Get_Children<Text>(this);
                var textList = textNodes.Select(txt => txt.data.AsMemory());
                return StringCommon.Concat(textList);
            }
            set
            {
                CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
                {
                    Node node = null;
                    if (!ReferenceEquals(null, value) && value.Length > 0)
                    {
                        node = new Text(nodeDocument, value);
                        Dom_replace_all_within_node(node, this);
                    }
                });
            }
        }
        #endregion
    }
}
