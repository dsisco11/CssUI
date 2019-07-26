using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Skips CR LF chars
    /// </summary>
    public class FilterCRLF : DataFilter<char>
    {
        public static FilterCRLF Instance = new FilterCRLF();
        public static string LF = new string(new char[] { UnicodeCommon.CHAR_LINE_FEED });
        public static string CRLF = new string(new char[] { UnicodeCommon.CHAR_CARRIAGE_RETURN, UnicodeCommon.CHAR_LINE_FEED });

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            switch (data)
            {
                case UnicodeCommon.CHAR_CARRIAGE_RETURN:
                case UnicodeCommon.CHAR_LINE_FEED:
                    {
                        return EFilterResult.FILTER_SKIP;
                    }
                default:
                    {
                        return EFilterResult.FILTER_ACCEPT;
                    }
            }
        }
    }
}
