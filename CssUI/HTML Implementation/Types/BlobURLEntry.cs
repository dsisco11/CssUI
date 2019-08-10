namespace CssUI.HTML
{
    public class BlobURLEntry
    {
        #region Properties
        public readonly dynamic Value;
        #endregion

        #region Constructors
        public BlobURLEntry(Blob Value)
        {
            this.Value = Value;
        }
        #endregion

        #region Implicit Casts
        public static implicit operator BlobURLEntry(Blob blob) => new BlobURLEntry(blob);
        #endregion
    }
}
