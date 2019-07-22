namespace CssUI.DOM
{
    /// <summary>
    /// The bdi element represents a span of text that is to be isolated from its surroundings for the purposes of bidirectional text formatting.
    /// </summary>
    public sealed class HTMLBdiElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-bdi-element */
        #region Constructor
        public HTMLBdiElement(Document document) : base(document, "bdi")
        {
        }
        #endregion

    }
}
