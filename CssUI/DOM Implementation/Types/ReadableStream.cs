using System;

namespace CssUI.DOM
{
    public class ReadableStream : DataStream<byte>
    {
        public ReadableStream(ReadOnlyMemory<byte> Data, byte EOF_ITEM) : base(Data, EOF_ITEM)
        {
        }

        public ReadableStream(byte[] Items, byte EOF_ITEM) : base(Items, EOF_ITEM)
        {
        }
    }
}
