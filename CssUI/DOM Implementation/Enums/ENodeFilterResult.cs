
namespace CssUI.DOM.Enums
{
    public enum ENodeFilterResult
    {
        /// <summary>
        /// Indicates a given value is valid and matches a particular filtering criteria
        /// </summary>
        FILTER_ACCEPT = 1,
        /// <summary>
        /// Indicates a given value is bad and not only does it not match the filtering criteria and that the filtering process should not continue
        /// </summary>
        FILTER_REJECT = 2,
        /// <summary>
        /// Indicates a given value is bad and does not match the filtering criteria and that the filtering process continue
        /// </summary>
        FILTER_SKIP = 3
    }
}
