
namespace CssUI.Types
{
    public class cssElementID : UniqueID
    {
        /// <summary>
        /// Tracks the sequence number for created IDs
        /// </summary>
        private static uint Sequence = 0;
        #region Accessors

        /// <summary>
        /// Gets or sets the seqential number of the ID.
        /// </summary>
        /// <value>
        /// The sequential count.
        /// </value>
        public uint SequenceNumber
        {
            get { return (uint)Bits[0, 0xFFFFF]; }
            private set { Bits[0, 0xFFFFF] = (ulong)value; }
        }


        /// <summary>
        /// </summary>
        public uint factor2
        {
            get { return (uint)Bits[50, 0xF]; }
            private set { Bits[50, 0xF] = (ulong)value; }
        }

        /// <summary>
        /// </summary>
        public uint factor3
        {
            get { return (uint)Bits[54, 0x3FF]; }
            set { Bits[54, 0x3FF] = (ulong)value; }
        }

        /// <summary>
        /// </summary>
        public uint factor4
        {
            get { return (uint)Bits[20, 0x3FFFFFFF]; }
            set { Bits[20, 0x3FFFFFFF] = (ulong)value; }
        }
        #endregion

        #region Constructor
        public cssElementID()
        {
            this.SequenceNumber = ++Sequence;
            this.factor2 = 0;
            this.factor3 = 0;
            this.factor4 = 0;
        }

        public cssElementID(ulong id) : base(id)
        {
        }
        #endregion
    }
}
