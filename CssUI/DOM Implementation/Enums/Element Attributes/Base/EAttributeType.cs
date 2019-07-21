namespace CssUI.DOM.Enums
{
    /// <summary>
    /// Specifies a DOM element attribute type, which determines what kind of string value formats are allowed to be set for it.
    /// </summary>
    public enum EAttributeType : int
    {
        /// <summary>
        /// Any string is valid
        /// </summary>
        String,
        /// <summary>
        /// A boolean attribute does not truly have a "value" its value is actually a <c>True</c>/<c>False</c> check of whether it is present or not (whether it is NULL)
        /// </summary>
        Boolean,
        /// <summary>
        /// Enumerated attributes may only be assigned a specific set of keywords with special meaning
        /// </summary>
        Enumerated,
        /// <summary>
        /// Numeric attributes may only be assigned a string representing a numeric value
        /// </summary>
        Numeric,

        /// <summary>
        /// KeyCombination attributes specify a combination of keypresses as a text string in a format defined in the HTML standards
        /// </summary>
        KeyCombo,
    }
}
