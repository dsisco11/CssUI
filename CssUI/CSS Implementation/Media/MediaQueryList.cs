using CssUI.CSS.Media;
using CssUI.CSS.Serialization;
using CssUI.DOM.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.DOM.Media
{
    public class MediaQueryList : EventTarget, ICssSerializeable, IDisposable
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#mediaquerylist */
        #region Properties
        public readonly Document document;
        public readonly LinkedList<MediaQuery> QueryList;
        public readonly string media;
        private bool oldMatchState = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Returns true if all of the media queries in this list match it's document.
        /// </summary>
        public bool Matches
        {
            get
            {
                foreach(MediaQuery query in QueryList)
                {
                    if (!query.Matches(document))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
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
            this.QueryList = queryList;
            this.media = CSS.Serialization.Serializer.MediaQueryList(QueryList);
            this.document._mediaQueryLists.AddLast(this);
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.document._mediaQueryLists.Remove(this);
                }
                disposedValue = true;
            }
        }
        #endregion

        internal void Evaluate()
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#evaluate-media-queries-and-report-changes */
            bool newMatchState = Matches;
            if (newMatchState != oldMatchState)
            {
                oldMatchState = newMatchState;
                var evt = new MediaQueryListEvent(EEventName.Change, new MediaQueryListEventInit(media, newMatchState));
                dispatchEvent( evt );
            }
        }

        public void addEventListener(EventListener listener)
        {
            if (listener == null)
            {
                return;
            }

            base.addEventListener(EEventName.Change, listener.callback, new AddEventListenerOptions(false));
        }

        public void removeEventListener(EventListener listener)
        {
            base.removeEventListener(EEventName.Change, listener.callback, new EventListenerOptions(false));
        }


        public string Serialize()
        {
            List<string> stringList = new List<string>();
            foreach (MediaQuery Query in QueryList)
            {
                stringList.Add(Query.Serialize());
            }

            return Serializer.Serialize_Comma_List(stringList);
        }
    }
}
