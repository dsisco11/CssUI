
using CssUI.CSS;

namespace CssUI.Internal
{
    /// <summary>
    /// Provides a function that can resolve a given property value into its next stage
    /// </summary>
    /// <param name="Property"></param>
    /// <param name="ComputedValue"></param>
    /// <returns></returns>
    public delegate dynamic PropertyResolverFunc(ICssProperty Property);


}
