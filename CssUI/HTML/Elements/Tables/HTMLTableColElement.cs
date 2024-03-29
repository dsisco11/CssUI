﻿using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// If a col element has a parent and that is a colgroup element that itself has a parent that is a table element, then the col element represents one or more columns in the column group represented by that colgroup.
    /// </summary>
    [MetaElement("col")]
    public class HTMLTableColElement : HTMLTableSectionElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#the-th-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Constructors
        public HTMLTableColElement(Document document) : base(document, "col")
        {
        }

        public HTMLTableColElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Number of columns spanned by the element
        /// </summary>
        [CEReactions]
        public uint span
        {
            get => MathExt.Clamp(getAttribute(EAttributeName.Span).AsUInt(), 1, 1000);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Span, AttributeValue.From(value)));
        }
        #endregion
    }
}
