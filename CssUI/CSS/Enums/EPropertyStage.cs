namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Describes all of the different value stages for a Css property
    /// </summary>
    public enum EPropertyStage : int
    {
        Invalid = 0,
        /// <summary>
        /// A value that has been declared in a stylesheet, it is likely one of many possibilities for its given property.
        /// </summary>
        Declared,
        /// <summary>
        /// Also known as the Cascaded value, this is the stage where a value has won out over all of the other defined for an element.
        /// </summary>
        Assigned,
        /// <summary>
        /// A value that has been interpreted through inheritence or defaulting.
        /// </summary>
        Specified,
        /// <summary>
        /// A value that has been interpreted to an actual number, rather then a percentage or function
        /// </summary>
        Computed,
        /// <summary>
        /// A value that has been further interpreted from the computed one, for example a computed value can be [width: auto], the used value is calculated from the computed to be an actual number
        /// </summary>
        Used,
        /// <summary>
        /// A value that can be used in the rendering process as it has been fully interpreted from the used value and any platform restrictions have been imposed on the value
        /// </summary>
        Actual
    }
}
