using System;
using System.Text;

namespace CssUI.Difference
{
    public struct DiffData<T> : IEquatable<DiffData<T>>
    {
        #region Properties
        private readonly int offset;
        private readonly ReadOnlyMemory<T> data;
        private readonly EDiffAction type;
        #endregion

        #region Accessors
        public int Offset => offset;
        public ReadOnlyMemory<T> Data => data;
        public EDiffAction Type => type;
        #endregion

        #region Constructors
        public DiffData(EDiffAction type, int offset, ReadOnlyMemory<T> data)
        {
            this.type = type;
            this.offset = offset;
            this.data = data;
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj is DiffData<T> other)
            {
                ((IEquatable<DiffData<T>>)this).Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(data, offset);
        }

        public static bool operator ==(DiffData<T> left, DiffData<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiffData<T> left, DiffData<T> right)
        {
            return !(left == right);
        }

        public bool Equals(DiffData<T> other)
        {
            return offset == other.Offset && data.Equals(other.Data);
        }

        public override string ToString()
        {
            StringBuilder Ret = new StringBuilder();

            foreach (T item in data.Span)
            {
                Ret.Append(item.ToString());
                //Ret.Append(" ");
            }

            return Ret.ToString();
        }
        #endregion
    }
}
