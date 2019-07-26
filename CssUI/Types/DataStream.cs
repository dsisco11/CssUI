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
        public int Position { get; private set; } = 0;

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
        public ItemType Peek(int Offset = 0)
        {
            int i = (Position + Offset);

            if (i < 0)
            {
                throw new IndexOutOfRangeException();
            }

            if (i >= Stream.Length)
            {
                return EOF_ITEM;
            }

            return Stream[i];
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public ItemType Consume()
        {
            int EndPos = (Position + 1);
            if (Position >= Stream.Length) return EOF_ITEM;

            ItemType retVal = Stream[Position];
            Position += 1;

            return retVal;
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public CastType Consume<CastType>() where CastType : ItemType
        {
            int EndPos = (Position + 1);
            if (Position >= Stream.Length) return default(CastType);

            ItemType retVal = Stream[Position];
            Position += 1;

            return (CastType)retVal;
        }

        /// <summary>
        /// Returns the specified number of items from the stream and progresses the current reading position by that number
        /// </summary>
        /// <param name="Count">Number of characters to consume</param>
        public ReadOnlySpan<ItemType> Consume(int Count = 1)
        {
            int startIndex = Position;
            int endIndex = (Position + Count);

            if (endIndex >= Stream.Length)
            {
                endIndex = (Stream.Length - 1);
            }

            Position = endIndex;
            return Stream.Slice(startIndex, Count);
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items and progresses the current reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public void Consume_While(Func<ItemType, bool> Predicate)
        {
            int startIndex = Position;

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
            int startIndex = Position;

            while (Predicate(Next))
            {
                Consume();
            }

            int count = Position - startIndex;
            outConsumed = Stream.Slice(startIndex, count);
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items as new stream and progresses this streams reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public DataStream<ItemType> Substream(Func<ItemType, bool> Predicate)
        {
            int startIndex = Position;

            while (Predicate(Next))
            {
                Consume();
            }

            int count = Position - startIndex;
            var consumed = Data.Slice(startIndex, count);

            return new DataStream<ItemType>(consumed, this.EOF_ITEM);
        }

        /// <summary>
        /// Pushes the given number of items back onto the front of the stream
        /// </summary>
        /// <param name="Count"></param>
        public void Reconsume(int Count = 1)
        {/* Docs: https://www.w3.org/TR/css-syntax-3/#reconsume-the-current-input-code-point */
            Position -= Count;
            if (Position < 0) Position = 0;
        }

        #endregion
    }
}
