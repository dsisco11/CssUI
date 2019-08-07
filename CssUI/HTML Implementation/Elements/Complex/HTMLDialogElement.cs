using CssUI.DOM;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System.Threading.Tasks;

namespace CssUI.HTML
{
    /// <summary>
    /// The dialog element represents a part of an application that a user interacts with to perform a task, for example a dialog box, inspector, or window.
    /// </summary>
    public sealed class HTMLDialogElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#the-dialog-element */

        /* XXX: If at any time a dialog element is removed from a Document, then if that dialog is in that Document's top layer, it must be removed from it. */

        #region Properties
        public string returnValue { get; set; } = string.Empty;
        public EDialogAlignmentMode Mode { get; private set; } = EDialogAlignmentMode.Normal;
        #endregion

        #region Constructor
        public HTMLDialogElement(Document document) : base(document, "dialog")
        {
        }

        public HTMLDialogElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// The open attribute is a boolean attribute. When specified, it indicates that the dialog element is active and that the user can interact with it.
        /// </summary>
        [CEReactions]
        public bool open
        {
            get => hasAttribute(EAttributeName.Open);
            set => CEReactions.Wrap_CEReaction(this, () => toggleAttribute(EAttributeName.Open, value));
        }
        #endregion

        internal void Set_Alignment_Mode(EDialogAlignmentMode mode)
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#centered-alignment */
            Mode = mode;
            if (mode == EDialogAlignmentMode.Normal)
            {
                /* XXX: Finish */
            }
            else if (mode == EDialogAlignmentMode.Centered)
            {
                /* XXX: Finish */
            }
        }


        [CEReactions]
        void show()
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#dom-dialog-show */

            CEReactions.Wrap_CEReaction(this, () =>
            {
                if (open) return;

                open = true;
                Set_Alignment_Mode(EDialogAlignmentMode.Normal);
                DOMCommon.Run_Dialog_Focusing_Steps(this);
            });
        }

        [CEReactions]
        void showModal()
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#dom-dialog-showmodal */
            CEReactions.Wrap_CEReaction(this, () =>
            {
                /* 1) Let subject be the dialog element on which the method was invoked. */
                /* 2) If subject already has an open attribute, then throw an "InvalidStateError" DOMException. */
                if (open) throw new InvalidStateError("The dialog element is already opened");
                /* 3) If subject is not connected, then throw an "InvalidStateError" DOMException. */
                if (!isConnected) throw new InvalidStateError("The dialog element is not connected to a document");
                /* 4) Add an open attribute to subject, whose value is the empty string. */
                open = true;
                /* 5) Set the dialog to the centered alignment mode. */
                Set_Alignment_Mode(EDialogAlignmentMode.Centered);
                /* 6) Let subject's node document be blocked by the modal dialog subject. */
                DOMCommon.Modal_Dialog_Block_Document(nodeDocument, this);
                /* 7) If subject's node document's top layer does not already contain subject, then add subject to subject's node document's top layer. */
                if (!nodeDocument.topLayer.Contains(this)) nodeDocument.topLayer.AddLast(this);
                /* 8) Run the dialog focusing steps for subject. */
                DOMCommon.Run_Dialog_Focusing_Steps(this);
            });
        }

        [CEReactions]
        void close(string result = null)
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#dom-dialog-close */
            CEReactions.Wrap_CEReaction(this, () =>
            {
                if (!open) return;
                open = false;
                if (!ReferenceEquals(null, result))
                {
                    returnValue = result;
                }

                DOMCommon.Modal_Dialog_Unblock_Document(nodeDocument, this);
                if (nodeDocument.topLayer.Contains(this))
                {
                    nodeDocument.topLayer.Remove(this);
                }

                Task.Factory.StartNew(() => dispatchEvent(new Event(EEventName.Close)));
            });
        }
    }
}
