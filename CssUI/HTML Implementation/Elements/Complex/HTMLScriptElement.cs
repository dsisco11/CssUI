using CssUI.DOM;
using CssUI.DOM.CustomElements;

namespace CssUI.HTML
{
    /// <summary>
    /// Please note that CSSUI will likely never support javascript, this class is only here as a courtesy
    /// </summary>
    public class HTMLScriptElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/scripting.html#the-script-element */
        #region Accessors
        [CEReactions] public string src;
        [CEReactions] public string type;
        [CEReactions] public bool noModule;
        [CEReactions] public bool async;
        [CEReactions] public bool defer;
        [CEReactions] public string crossOrigin;
        [CEReactions] public string text;
        [CEReactions] public string integrity;
        [CEReactions] public string referrerPolicy;
        #endregion

        #region Construcotrs
        public HTMLScriptElement(Document document) : base(document, "script")
        {
        }

        public HTMLScriptElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion


    }
}
