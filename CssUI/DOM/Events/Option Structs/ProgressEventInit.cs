namespace CssUI.DOM.Events
{
    public class ProgressEventInit : EventInit
    {
        #region Properties
        public readonly bool lengthComputable;
        public readonly ulong loaded;
        public readonly ulong total;
        #endregion

        #region Constructor
        public ProgressEventInit(bool lengthComputable, ulong loaded, ulong total)
        {
            this.lengthComputable = lengthComputable;
            this.loaded = loaded;
            this.total = total;
        }
        #endregion


    }
}
