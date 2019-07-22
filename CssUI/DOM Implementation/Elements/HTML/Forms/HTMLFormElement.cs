using CssUI.DOM.CustomElements;

namespace CssUI.DOM
{
    public class HTMLFormElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/forms.html#htmlformelement */
        #region Properties
        /*[CEReactions] public string acceptCharset;
        [CEReactions] public string action;
        [CEReactions] public string autocomplete;
        [CEReactions] public string enctype;
        [CEReactions] public string encoding;
        [CEReactions] public string method;
        [CEReactions] public string name;
        [CEReactions] public bool noValidate;
        [CEReactions] public string target;
        [CEReactions] public string rel;
        public readonly DOMTokenList relList;

        public readonly HTMLFormControlsCollection elements;
        public readonly ulong length;
        public getter Element(ulong index);
        public getter(RadioNodeList or Element) (string name);

        public void submit();
        public void requestSubmit(HTMLElement submitter);

        [CEReactions]
        public void reset();
        public bool checkValidity();
        public bool reportValidity();*/
        #endregion

        #region Constructor
        public HTMLFormElement(Document document) : base(document, "form")
        {
        }

        public HTMLFormElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
