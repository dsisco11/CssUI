
using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    /// <summary>
    /// Accepts any selectable unicode code points
    /// </summary>
    public class FilterCharSelectable : Filter<char>
    {
        public static Filter<char> Instance = new FilterCharSelectable();

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
