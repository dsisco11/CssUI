using CssUI.HTTP;
using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Skips HTTP whitespace chars
    /// </summary>
    public class FilterHTTPWhitespace : DataFilter<char>
    {
        public static DataFilter<char> Instance = new FilterHTTPWhitespace();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            if (HTTPCommon.Is_HTTP_Whitespace(data))
            {
                return EFilterResult.FILTER_SKIP;
            }

            return EFilterResult.FILTER_ACCEPT;
        }
    }
}
