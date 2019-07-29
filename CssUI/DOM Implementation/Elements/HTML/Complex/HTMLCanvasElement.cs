namespace CssUI.DOM
{
    /* XXX: Implement canvas element */
    public class HTMLCanvasElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/canvas.html#the-canvas-element */
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
