
namespace CssUI.CSS
{
    public enum ECssAttributeOperator
    {
        /// <summary></summary>
        None,
        /// <summary>Matches if the attribute has ANY set value</summary>
        Isset,
        /// <summary>Matches if the attribute is exactly equal to our value</summary>
        Equals,
        /// <summary>Matches values equal to our own or which are prefixed with "{ourValue}-" </summary>
        PrefixedWith,
        /// <summary>Matches if our value is present in the attribute value when viewed as a space-seperated list of values</summary>
        Includes,
        /// <summary>Substring starts with</summary>
        StartsWith,
        /// <summary>Substring ends with</summary>
        EndsWith,
        /// <summary>Substring contains</summary>
        Contains,
    }
}
