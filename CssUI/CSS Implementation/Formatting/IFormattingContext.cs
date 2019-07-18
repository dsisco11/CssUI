using CssUI.DOM;
using System.Collections.Generic;

namespace CssUI.CSS.Formatting
{
    public interface IFormattingContext
    {
        /// <summary>
        /// Performs layout on all elements within the target, returning the generated box fragments for rendering.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<CssBoxFragment> Flow(Element target);
    }
}
