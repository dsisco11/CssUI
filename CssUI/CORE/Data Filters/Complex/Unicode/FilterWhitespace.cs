using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Skips ASCII whitespace chars
    /// </summary>
    public class FilterWhitespace : Filter<char>
    {
        public static Filter<char> Instance = new FilterWhitespace();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            if (UnicodeCommon.Is_Ascii_Whitespace(data))
            {
                return EFilterResult.FILTER_SKIP;
            }

            return EFilterResult.FILTER_ACCEPT;
        }
    }
}
