using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace CssUI.Difference
{
    public class DiffNode<T>// where T : IEquatable<T>
    {
        #region Properties
        private readonly int startOffset;
        private readonly int endOffset;
        private readonly EDiffAction type;
        private readonly ReadOnlyMemory<T> data;
        //private readonly WeakReference context;
        #endregion

        #region Accessors
        public int Length => EndOffset - StartOffset;

        public Int32 StartOffset => startOffset;
        public Int32 EndOffset => endOffset;
        public EDiffAction Type => type;
        public ReadOnlyMemory<T> Data => data;
        #endregion

        #region Constructors
        public DiffNode(ReadOnlyMemory<T> data, int start, int end, EDiffAction type)
        {
            //startOffset = MathExt.Min(offset1, offset2);
            //endOffset = MathExt.Max(offset1, offset2);
            startOffset = start;
            endOffset = end;
            this.data = data.Slice(startOffset, Length);
            this.type = type;
        }

        /*public DiffNode(object context, int start, int end, EDiffAction type)
        {
            this.context = new WeakReference(context);
            startOffset = start;
            endOffset = end;
            this.type = type;
        }*/
        #endregion

        #region Overrides
        //public override string ToString() => $"{type}<{startOffset}, {endOffset}>";
        public override string ToString()
        {
            if (data.IsEmpty)
            {
                return $"{type}<{startOffset}, {endOffset}>";
            }

            StringBuilder buf = new StringBuilder();
            switch (type)
            {
                case EDiffAction.Insertion:
                    buf.Append("+ ");
                    break;
                case EDiffAction.Removal:
                    buf.Append("- ");
                    break;
                case EDiffAction.Modify:
                    buf.Append("~ ");
                    break;
            }

            for (int i=0; i<data.Length; i++)
            {
                T item = data.Span[i];
                buf.Append( item.ToString() );
                if (i < (data.Length-1) )
                    buf.Append(", ");
            }

            return buf.ToString();
            //if (!context.IsAlive) throw new ObjectDisposedException(nameof(context));
            //DataConsumer<T> Stream = (DataConsumer<T>)context.Target;
            //return Stream.AsMemory().Slice(startOffset, Length).ToString();
        }
        #endregion
    }
}
