
using CssUI.DOM.Enums;
using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// The basic interface, from which all the HTML elements' interfaces inherit, and which must be used by elements that have no additional requirements, is the HTMLElement interface.
    /// </summary>
    public class HTMLElement : Element, IGlobalEventHandlers, IDocumentAndElementEventHandlers
    {
        # region Metadata Attributes
        public string title;
        public string lang;
        public bool translate;
        public string dir;
        public readonly DOMStringMap dataset;
        public string nonce;

        /// <summary>
        /// Specifies the tab index of this element, that is its selection order when a user cycles through selecting elements by pressing tab
        /// </summary>
        public long tabIndex;
        #endregion

        #region Constructors
        public HTMLElement(Document document, string localName) : base(document, localName)
        {
            dataset = new DOMStringMap(this);
        }
        #endregion


        #region User Interaction
        public bool hidden;
        public void click();
        public void focus(FocusOptions options);
        // public void blur(); /* "User agents are encouraged to ignore calls to this blur() method entirely." - https://html.spec.whatwg.org/multipage/interaction.html#dom-window-blur */
        public string accessKey;
        public string accessKeyLabel { get; private set; }
        public bool draggable;
        public bool spellcheck;
        public string autocapitalize;

        public string innerText;
        #endregion

        #region Content Editable
        public string contentEditable { get; set; }
        public string enterKeyHint { get; set; }
        public bool isContentEditable { get; set; }
        public string inputMode { get; set; }
        #endregion

        #region Document Event Handlers
        public event EventCallback onCopy
        {
            add => handlerMap.Add(EEventName.Copy, value);
            remove => handlerMap.Remove(EEventName.Copy, value);
        }

        public event EventCallback onCut
        {
            add => handlerMap.Add(EEventName.Cut, value);
            remove => handlerMap.Remove(EEventName.Cut, value);
        }

        public event EventCallback onPaste
        {
            add => handlerMap.Add(EEventName.Paste, value);
            remove => handlerMap.Remove(EEventName.Paste, value);
        }
        #endregion

    }

}
