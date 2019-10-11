namespace CssUI.DOM.Events
{
    public class ProgressEvent : Event
    {/* Docs: https://xhr.spec.whatwg.org/#progressevent */
        #region Properties
        public readonly bool lengthComputable;
        public readonly ulong loaded;
        public readonly ulong total;
        #endregion

        #region Constructors
        public ProgressEvent(EventName type, ProgressEventInit initDict = null) : base(type, initDict)
        {
            lengthComputable = initDict.lengthComputable;
            loaded = initDict.loaded;
            total = initDict.total;
        }
        #endregion

    }
}
