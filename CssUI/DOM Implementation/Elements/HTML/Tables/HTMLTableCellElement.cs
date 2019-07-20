namespace CssUI.DOM
{
    public class HTMLTableCellElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/tables.html#htmltablecellelement */
        #region Properties
        [CEReactions] public ulong colSpan;
        [CEReactions] public ulong rowSpan;
        [CEReactions] public string headers;
        public readonly long cellIndex;

        [CEReactions] public string scope; // only conforming for th elements
        [CEReactions] public string abbr;  // only conforming for th elements
        #endregion

        #region Constructor
        public HTMLTableCellElement(Document document, string localName, string prefix, string Namespace) : base(document, localName, prefix, Namespace)
        {
        }
        #endregion
    }
}
