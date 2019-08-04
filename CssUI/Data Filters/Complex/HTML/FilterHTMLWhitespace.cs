using CssUI.HTML;
using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Skips ASCII whitespace chars
    /// </summary>
    public class FilterHTMLWhitespace : DataFilter<char>
    {
        public static DataFilter<char> Instance = new FilterHTMLWhitespace();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            if (HTMLCommon.Is_HTML_Whitespace(data))
            {
                return EFilterResult.FILTER_SKIP;
            }

            return EFilterResult.FILTER_ACCEPT;
        }
    }
}
