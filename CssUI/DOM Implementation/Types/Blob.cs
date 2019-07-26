using CssUI.DOM.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CssUI.DOM
{

    public class Blob : ICollection<byte>
    {/* Docs: https://w3c.github.io/FileAPI/#dfn-Blob */
        #region Properties
        public readonly ulong size;
        public readonly string type;
        protected byte[] data;
        #endregion

        #region Constructor
        public Blob(IReadOnlyCollection<ReadOnlyMemory<byte>> blobParts, BlobPropertyBag options = null)
        {
        }
        #endregion


        #region Memory
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(data);

        public ReadOnlyMemory<byte> AsMemory() => new ReadOnlyMemory<byte>(data);
        #endregion


        // slice Blob into byte-ranged chunks
        public Blob slice(long start, long end, string contentType)
        {

        }

        // read from the Blob.
        public ReadableStream stream();
        public TaskCompletionSource<string> text();
        public TaskCompletionSource<byte[]> arrayBuffer();



        #region ICollection Implementation
        public int Count => ((ICollection<byte>)data).Count;

        public bool IsReadOnly => ((ICollection<byte>)data).IsReadOnly;

        public void Add(byte item)
        {
            ((ICollection<byte>)data).Add(item);
        }

        public void Clear()
        {
            ((ICollection<byte>)data).Clear();
        }

        public bool Contains(byte item)
        {
            return ((ICollection<byte>)data).Contains(item);
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            ((ICollection<byte>)data).CopyTo(array, arrayIndex);
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return ((ICollection<byte>)data).GetEnumerator();
        }

        public bool Remove(byte item)
        {
            return ((ICollection<byte>)data).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<byte>)data).GetEnumerator();
        }
        #endregion

    }
}
