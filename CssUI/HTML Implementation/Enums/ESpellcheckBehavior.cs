namespace CssUI.DOM
{
    /// <summary>
    /// Determines the default behavior for an elements spellchecking
    /// </summary>
    public enum ESpellcheckBehavior : int
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#spelling-and-grammar-checking */

        /// <summary>
        /// The element will be checked for spelling and grammar if its contents are editable and spellchecking is not explicitly disabled through the spellcheck attribute.
        /// </summary>
        True_By_Default = -3,

        /// <summary>
        /// The element will never be checked for spelling and grammar unless spellchecking is explicitly enabled through the spellcheck attribute.
        /// </summary>
        False_By_Default = -2,

        /// <summary>
        /// The element's default behavior is the same as its parent element's. Elements that have no parent element cannot have this as their default behavior.
        /// </summary>
        Inherit_By_Default = -1,
    }
}
