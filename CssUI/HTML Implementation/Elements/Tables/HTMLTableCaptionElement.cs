﻿using CssUI.DOM;

namespace CssUI.HTML
{
    public class HTMLTableCaptionElement : HTMLElement
    {
        #region Constructors
        public HTMLTableCaptionElement(Document document) : base(document, "caption")
        {
        }
        public HTMLTableCaptionElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
