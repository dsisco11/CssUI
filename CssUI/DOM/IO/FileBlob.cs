using System;
using System.Collections;
using System.Collections.Generic;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents files data as well as its name and the time it was last modified
    /// </summary>
    public class FileBlob : Blob
    {
        #region Properties
        public readonly string name;
        public readonly ulong lastModified = ulong.MaxValue;
        #endregion

        #region Constructors
        public FileBlob(ReadOnlyMemory<byte> data, string name, FilePropertyBag options = null) : base(new BlobPart[] { data }, options)
        {
            this.name = name;
            if (ReferenceEquals(null, data) || data.IsEmpty)
            {
                this.data = Array.Empty<byte>();
            }
            else
            {
                this.data = new byte[data.Length];
                var dataMem = new Memory<byte>(this.data);

                data.CopyTo(dataMem);
            }
        }

        #endregion

    }
}
