using CssUI.CSS.Media;
using CssUI.DOM.Events;
using System.Collections.Generic;

namespace CssUI.DOM.Media
{
    public class MediaQueryList : EventTarget
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#mediaquerylist */
        #region Properties
        public readonly Document document;
        public readonly LinkedList<MediaQuery> QueryList;
        public readonly string media;
        public readonly bool matches;
        #endregion

        #region Events
        public event EventCallback onchange
        {
            add => handlerMap.Add(EEventName.Change, value);
            remove => handlerMap.Remove(EEventName.Change, value);
        }
        #endregion

        #region Constructors

        public MediaQueryList(Document document, LinkedList<MediaQuery> queryList)
        {
            this.document = document;
            QueryList = queryList;
        }
        #endregion

        public void addEventListener(EventListener listener)
        {
            if (ReferenceEquals(null, listener))
                return;

            base.addEventListener(EEventName.Change, listener.callback, new AddEventListenerOptions(false));
        }

        public void removeEventListener(EventListener listener)
        {
            base.removeEventListener(EEventName.Change, listener.callback, new EventListenerOptions(false));
        }
    }
}
