using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The template element is used to declare fragments of HTML that can be cloned and inserted in the document by script.
    /// </summary>
    [MetaElement("template")]
    public sealed class HTMLTemplateElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#the-template-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.MetaData | EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.ScriptSupporting;
        #endregion

        #region Properties
        public readonly DocumentFragment content;
        #endregion

        #region Constructor
        public HTMLTemplateElement(Document document, DocumentFragment content) : this(document, content, "template")
        {
        }

        public HTMLTemplateElement(Document document, DocumentFragment content, string localName) : base(document, localName)
        {
            this.content = content;
        }
        #endregion

    }
}
