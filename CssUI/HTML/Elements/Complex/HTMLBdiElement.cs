using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The bdi element represents a span of text that is to be isolated from its surroundings for the purposes of bidirectional text formatting.
    /// </summary>
    [MetaElement("bdi")]
    public sealed class HTMLBdiElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-bdi-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Palpable;
        #endregion

        #region Constructor
        public HTMLBdiElement(Document document) : base(document, "bdi")
        {
        }
        #endregion

    }
}
