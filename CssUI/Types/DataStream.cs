using System;
using System.Runtime.CompilerServices;

namespace CssUI
{
    /// <summary>
    /// Provides access to a genericized, consumable stream of data.
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public class DataStream<ItemType>
    {
        #region Properties
        /// <summary>
        /// Our stream of tokens
        /// </summary>
        private readonly ReadOnlyMemory<ItemType> Data;
        private ReadOnlySpan<ItemType> Stream => Data.Span;

        /// <summary>
        /// The current position at which data will be read from the stream
        /// </summary>
        public ulong Position { get; private set; } = 0;

        public readonly ItemType EOF_ITEM = default(ItemType);
        #endregion

        #region Constructors
        public DataStream(ReadOnlyMemory<ItemType> Data, ItemType EOF_ITEM)
        {
            this.Data = Data;
            this.EOF_ITEM = EOF_ITEM;
        }

        public DataStream(ItemType[] Items, ItemType EOF_ITEM)
        {
            this.Data = new ReadOnlyMemory<ItemType>(Items);
            this.EOF_ITEM = EOF_ITEM;
        }
        #endregion

        #region Accessors
        public int Length => Data.Length;
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(0)
        /// </summary>
        public ItemType Next => Peek(0);
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(1)
        /// </summary>
        public ItemType NextNext => Peek(1);
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(2)
        /// </summary>
        public ItemType NextNextNext => Peek(2);

        /// <summary>
        /// Returns whether the stream position is currently at the EOF
        /// </summary>
        public bool atEOF => Peek(0).Equals(EOF_ITEM);
        #endregion

        #region Data
        public ReadOnlyMemory<ItemType> AsMemory() => Data;
        public ReadOnlySpan<ItemType> AsSpan() => Data.Span;
        #endregion

        #region Stream Management
        /// <summary>
        /// Returns the item at +<paramref name="Offset"/> from the current read position
        /// </summary>
        /// <param name="Offset">Distance from the current read position at which to peek</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemType Peek(long Offset = 0)
        {
            long index = ((long)Position + Offset);

            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (index >= Stream.Length)
            {
                return EOF_ITEM;
            }

            return Stream[(int)index];
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public ItemType Consume()
        {
            var EndPos = (Position + 1);
            if (Position >= (ulong)Stream.Length) return EOF_ITEM;

            ItemType retVal = Stream[(int)Position];
            Position += 1;

            return retVal;
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public CastType Consume<CastType>() where CastType : ItemType
        {
            var EndPos = (Position + 1);
            if (Position >= (ulong)Stream.Length) return default(CastType);

            ItemType retVal = Stream[(int)Position];
            Position += 1;

            return (CastType)retVal;
        }

        /// <summary>
        /// Returns the specified number of items from the stream and progresses the current reading position by that number
        /// </summary>
        /// <param name="Count">Number of characters to consume</param>
        public ReadOnlySpan<ItemType> Consume(ulong Count = 1)
        {
            var startIndex = Position;
            var endIndex = (Position + Count);

            if (endIndex >= (ulong)Stream.Length)
            {
                endIndex = (((ulong)Stream.Length) - 1);
            }

            Position = endIndex;
            return Stream.Slice((int)startIndex, (int)Count);
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items and progresses the current reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public void Consume_While(Func<ItemType, bool> Predicate)
        {
            while (Predicate(Next))
            {
                Consume();
            }
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items and progresses the current reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public void Consume_While(Func<ItemType, bool> Predicate, out ReadOnlySpan<ItemType> outConsumed)
        {
            var startIndex = Position;

            while (Predicate(Next))
            {
                Consume();
            }

            var count = Position - startIndex;
            outConsumed = Stream.Slice((int)startIndex, (int)count);
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items as new stream and progresses this streams reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public DataStream<ItemType> Substream(Func<ItemType, bool> Predicate)
        {
            var startIndex = Position;

            while (Predicate(Next))
            {
                Consume();
            }

            var count = Position - startIndex;
            var consumed = Data.Slice((int)startIndex, (int)count);

            return new DataStream<ItemType>(consumed, EOF_ITEM);
        }

        /// <summary>
        /// Pushes the given number of items back onto the front of the stream
        /// </summary>
        /// <param name="Count"></param>
        public void Reconsume(ulong Count = 1)
        {
            Position -= Count;
        }

        #endregion
    }
}
