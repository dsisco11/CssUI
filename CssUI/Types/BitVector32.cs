using System;

namespace CssUI
{
    public class BitVector32
    {
        private UInt32 data;

        public BitVector32()
        {
        }
        public BitVector32(UInt32 value)
        {
            data = value;
        }

        public UInt32 Data
        {
            get { return data; }
            set { data = value; }
        }

        public UInt32 this[uint bitoffset, UInt32 valuemask]
        {
            get
            {
                return (data >> (ushort)bitoffset) & valuemask;
            }
            set
            {
                data = (data & ~(valuemask << (ushort)bitoffset)) | ((value & valuemask) << (ushort)bitoffset);
            }
        }
    }
}
