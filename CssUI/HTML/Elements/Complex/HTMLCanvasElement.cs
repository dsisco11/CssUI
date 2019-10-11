using CssUI.DOM;

namespace CssUI.HTML
{
    /* XXX: Implement canvas element */
    [MetaElement("canvas")]
    public class HTMLCanvasElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/canvas.html#the-canvas-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Embedded | EContentCategories.Palpable;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public HTMLCanvasElement(Document document) : base(document, "canvas")
        {
        }

        public HTMLCanvasElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


    }
}
