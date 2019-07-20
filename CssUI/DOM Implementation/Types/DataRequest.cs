using CssUI.DOM.Enums;
using System.IO;
using System.Threading;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents a request for data of some sort, it could be from a local file or from the web.
    /// </summary>
    public abstract class DataRequest
    {/* Docs: https://html.spec.whatwg.org/multipage/images.html#image-request */
        #region Properties
        public readonly string currentURL = null;
        public MemoryStream Data { get; protected set; } = null;
        /// <summary>
        /// Awaitable event signaling a state change for this data request
        /// </summary>
        public readonly AutoResetEvent State_Change_Signal = new AutoResetEvent(false);

        private volatile EDataRequestState _state = EDataRequestState.Unavailable;
        public EDataRequestState State
        {
            get => _state;
            protected set
            {
                if (value != _state)
                {
                    _state = value;
                    State_Change_Signal.Set();
                }
            }
        }
        #endregion

        #region Constructor
        public DataRequest(string currentURL)
        {
            this.currentURL = currentURL;
        }
        #endregion

        /// <summary>
        /// Begins the data request
        /// </summary>
        public abstract void Fetch();
    }
}
