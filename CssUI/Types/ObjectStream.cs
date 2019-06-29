using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Provides access to a genericized, consumable stream of items.
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public class ObjectStream<ItemType> where ItemType : class
    {
        #region Properties
        /// <summary>
        /// Our stream of tokens
        /// </summary>
        readonly ItemType[] Stream = null;
        /// <summary>
        /// The current position at which tokens will be read from the stream
        /// </summary>
        int ReadPos = 0;

        public readonly ItemType EOF_ITEM = default(ItemType);
        #endregion

        #region Constructors
        public ObjectStream(ItemType[] Items, ItemType EOF_ITEM)
        {
            this.Stream = Items;
            this.EOF_ITEM = EOF_ITEM;
        }

        public ObjectStream(IEnumerable<ItemType> Items, ItemType EOF_ITEM)
        {
            this.Stream = Items.ToArray();
            this.EOF_ITEM = EOF_ITEM;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(0)
        /// </summary>
        public ItemType Next { get { return Peek(0); } }
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(1)
        /// </summary>
        public ItemType NextNext { get { return Peek(1); } }
        /// <summary>
        /// Returns the next item to be consumed, equivalent to calling Peek(2)
        /// </summary>
        public ItemType NextNextNext { get { return Peek(2); } }
        #endregion

        #region Stream Management
        /// <summary>
        /// Returns the character at +Offset from the current read position within the text
        /// </summary>
        /// <param name="Offset">Distance from the current read position at which to peek</param>
        /// <returns></returns>
        public ItemType Peek(int Offset = 0)
        {
            int i = (ReadPos + Offset);
            if (i >= Stream.Length || i < 0) return EOF_ITEM;
            return Stream[i];
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public ItemType Consume()
        {
            int EndPos = (ReadPos + 1);
            if (ReadPos >= Stream.Length) return EOF_ITEM;

            ItemType retVal = Stream[ReadPos];
            ReadPos += 1;

            return retVal;
        }

        /// <summary>
        /// Returns the first unconsumed item from the stream and progresses the current reading position
        /// </summary>
        public CastType Consume<CastType>() where CastType : ItemType
        {
            int EndPos = (ReadPos + 1);
            if (ReadPos >= Stream.Length) return default(CastType);

            ItemType retVal = Stream[ReadPos];
            ReadPos += 1;

            return (CastType)retVal;
        }

        /// <summary>
        /// Returns the specified number of items from the stream and progresses the current reading position by that number
        /// </summary>
        /// <param name="Count">Number of characters to consume</param>
        public List<ItemType> Consume(int Count = 1)
        {
            int EndPos = (ReadPos + Count);
            if (EndPos >= Stream.Length) EndPos = (Stream.Length - 1);

            List<ItemType> retVal = new List<ItemType>();
            for (int i = ReadPos; i < EndPos; i++)
            {
                retVal.Add(Stream[i]);
            }
            ReadPos = EndPos;

            return retVal;
        }

        /// <summary>
        /// Consumes items until reaching the first one that does not match the given predicate, then returns all matched items and progresses the current reading position by that number
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        public List<ItemType> Consume_While(Func<ItemType, bool> Predicate)
        {
            List<ItemType> Matches = new List<ItemType>();
            
            while (Predicate(Next))
            {
                Matches.Add(Consume());
            }

            ReadPos += Matches.Count;
            return Matches;
        }

        /// <summary>
        /// Pushes the given number of items back onto the front of the stream
        /// </summary>
        /// <param name="Count"></param>
        public void Reconsume(int Count = 1)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#reconsume-the-current-input-code-point
            ReadPos -= Count;
            if (ReadPos < 0) ReadPos = 0;
        }

        #endregion
    }
}
