using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    public abstract class Filter<Ty>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract EFilterResult acceptData(Ty data);
    }
}
