using System;

namespace CssUI.DOM
{
    public class BlobPart
    {
        #region Properties
        public readonly dynamic data;
        #endregion


        #region Constructors
        public BlobPart(ReadOnlyMemory<byte> data)
        {
            this.data = data;
        }
        public BlobPart(Blob data)
        {
            this.data = data;
        }
        public BlobPart(string data)
        {
            this.data = data;
        }
        #endregion

        #region Implicit
        public static implicit operator BlobPart(ReadOnlyMemory<byte> data) => new BlobPart(data);
        public static implicit operator BlobPart(Blob blob) => new BlobPart(blob);
        public static implicit operator BlobPart(string str) => new BlobPart(str);
        #endregion
    }
}
