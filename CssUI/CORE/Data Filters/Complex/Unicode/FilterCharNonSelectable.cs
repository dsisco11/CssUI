
using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Rejects any non-selectable unicode code points
    /// </summary>
    public class FilterCharNonSelectable : Filter<char>
    {
        public static Filter<char> Instance = new FilterCharNonSelectable();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EFilterResult acceptData(char data)
        {
            if (UnicodeCommon.Is_Selectable_Char(data))
            {
                return EFilterResult.FILTER_ACCEPT;
            }

            return EFilterResult.FILTER_REJECT;
        }
    }
}
