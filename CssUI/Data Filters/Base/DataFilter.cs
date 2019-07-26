using System.Runtime.CompilerServices;

namespace CssUI.Filters
{
    public abstract class DataFilter<Ty>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract EFilterResult acceptData(Ty data);
    }
}
