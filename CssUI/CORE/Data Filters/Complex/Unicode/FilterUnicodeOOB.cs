using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Filters any unicode characters which exceed 0xFFFF
    /// </summary>
    public class FilterUnicodeOOB : Filter<char>
    {
        public static Filter<char> Instance = new FilterUnicodeOOB();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            if (data > 0xFFFF)
                return EFilterResult.FILTER_SKIP;

            return EFilterResult.FILTER_ACCEPT;
        }
    }
}
