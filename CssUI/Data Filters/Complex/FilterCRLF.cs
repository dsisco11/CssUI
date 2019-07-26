
using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Filters CR LF chars, rejecting CR and skipping LF
    /// </summary>
    public class FilterCRLF : DataFilter<DataStream<char>>
    {
        public static FilterCRLF Instance = new FilterCRLF();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(DataStream<char> Stream)
        {
            switch (Stream.Next)
            {
                case UnicodeCommon.CHAR_CARRIAGE_RETURN:
                    {
                        return EFilterResult.FILTER_REJECT;
                    }
                case UnicodeCommon.CHAR_LINE_FEED:
                    {
                        /* LF must come after CR otherwise we want to replace it with CRLF */
                        if (Stream.Peek(-1) != UnicodeCommon.CHAR_CARRIAGE_RETURN)
                        {// this LF is all alone, so we want to replace it with a full CRLF, reject it to spawn a new chunk
                            return EFilterResult.FILTER_REJECT;
                        }
                        else
                        {// otherwise this LF comes after a CR so we just want to exclude it from the next chunk
                            return EFilterResult.FILTER_SKIP;
                        }
                    }
                default:
                    {
                        return EFilterResult.FILTER_ACCEPT;
                    }
            }
        }
    }
}
