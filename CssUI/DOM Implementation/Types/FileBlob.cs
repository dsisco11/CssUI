using System;
using System.Collections;
using System.Collections.Generic;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents files data as well as its name and the time it was last modified
    /// </summary>
    public class FileBlob : ICollection<byte>
    {
        #region Properties
        public readonly string name;
        public readonly ulong lastModified = ulong.MaxValue;
        private byte[] data;
        #endregion

        #region Constructors
        public FileBlob(string name, ReadOnlyMemory<byte> data)
        {
            this.name = name;
            if (ReferenceEquals(null, data) || data.IsEmpty)
            {
                this.data = new byte[0];
            }
            else
            {
                this.data = new byte[data.Length];
                var dataMem = new Memory<byte>(this.data);

                data.CopyTo(dataMem);
            }
        }

        public FileBlob(string name, ReadOnlyMemory<byte> data, ulong lastModified)
        {
            this.name = name;
            this.lastModified = lastModified;

            if (ReferenceEquals(null, data) || data.IsEmpty)
            {
                this.data = new byte[0];
            }
            else
            {
                this.data = new byte[data.Length];
                var dataMem = new Memory<byte>(this.data);

                data.CopyTo(dataMem);
            }
        }
        #endregion

        #region Memory
        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(data);

        public ReadOnlyMemory<byte> AsMemory() => new ReadOnlyMemory<byte>(data);
        #endregion

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
