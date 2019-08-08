using CssUI.DOM.Events;
using System;
using System.Collections.Generic;

namespace CssUI.DOM.Internal
{
    /// <summary>
    /// A browsing context is like an abstract way for a document to interact with the environment its being presented from.
    /// Because a document could be just in a normal window or possibly embedded within a control element in which case it isnt directly contained in a window but still is owned by one, so it needs to reference the window object at its' root.
    /// <para>"A browsing context is an environment in which Document objects are presented to the user."</para>
    /// </summary>
    public abstract class BrowsingContext : EventTarget
    {
        #region Properties
        //public Document document { get; protected set; }
        public readonly WeakReference<BrowsingContext> _opener = new WeakReference<BrowsingContext>(null);
        public bool Disowned { get; protected set; } = false;
        public bool IsClosing { get; protected set; } = false;

        /* 
         * A browsing context has a session history, which lists the Document objects that the browsing context has presented, is presenting, or will present. 
         * A browsing context's active document is its WindowProxy object's [[Window]] internal slot value's associated Document. 
         * A Document's browsing context is the browsing context whose session history contains the Document, if any such browsing context exists and has not been discarded, and null otherwise. 
         */
        public readonly LinkedList<Document> SessionHistory = new LinkedList<Document>();

        #endregion

        #region Constructors
        protected BrowsingContext(BrowsingContext opener = null)
        {
            _opener.SetTarget(opener);
        }
        #endregion

        #region Accessors
        public bool IsTopLevel
        {
            get => (Opener == null);
        }

        public Document activeDocument
        {/* A browsing context's active document is its WindowProxy object's [[Window]] internal slot value's associated Document. */
            get
            {
                return WindowProxy?.document;
            }
        }

        /* A browsing context has a corresponding WindowProxy object. */
        internal abstract Window WindowProxy { get; }

        public BrowsingContext Opener
        {/* A browsing context has an opener browsing context, which is null or a browsing context. It is initially null. */
            get
            {
                if (_opener.TryGetTarget(out BrowsingContext outOpener))
                    return outOpener;

                return null;
            }
        }
        #endregion


        public BrowsingContext Get_Top_Level_Browsing_Context()
        {
            BrowsingContext context = this;

            while (!context.IsTopLevel)
            {
                context = context.Opener;
            }

            return context;
        }
    }
}
