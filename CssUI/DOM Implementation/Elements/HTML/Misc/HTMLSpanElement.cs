﻿namespace CssUI.DOM
{
    /// <summary>
    /// The span element doesn't mean anything on its own, but can be useful when used together with the global attributes, e.g. class, lang, or dir. It represents its children.
    /// </summary>
    public sealed class HTMLSpanElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-span-element */

        #region Constructor
        public HTMLSpanElement(Document document) : base(document, "span")
        {
        }

        public HTMLSpanElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

    }
}
