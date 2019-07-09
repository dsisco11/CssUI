namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Defines all the simple selector types 
    /// </summary>
    public enum ECssSimpleSelectorType
    {
        /// <summary>The universal selector matches any element in any namespace</summary>
        UniversalSelector,
        /// <summary>A type selector is the name of an element type written using the syntax of CSS qualified names SEE: https://www.w3.org/TR/css3-namespace/#css-qnames </summary>
        TypeSelector,
        /// <summary>Matches an element attribute value</summary>
        AttributeSelector,
        /// <summary>Matches an element styling class</summary>
        ClassSelector,
        /// <summary>Matches the unique ID of an element</summary>
        IDSelector,
        /// <summary>Matches pseudo classes, which are element states based on information not contained within the UI system</summary>
        PseudoClassSelector,
        /// <summary>Matches pseudo element, which are elements that are not represented by a seperate element class instance, and are instead a sub-part of an element. EG: the "::first-line" or "::first-letter"</summary>
        PseudoElementSelector
    }
}
