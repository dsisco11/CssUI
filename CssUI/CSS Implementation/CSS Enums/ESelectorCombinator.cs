
namespace CssUI.CSS.Internal
{
    public enum ESelectorCombinator
    {/* Docs: https://www.w3.org/TR/2018/REC-selectors-3-20181106/#combinators */
        None = 0,
        /// <summary>
        /// '>>' or ' '
        /// Descendant combinators express such a relationship. 
        /// A descendant combinator is whitespace that separates two sequences of simple selectors. 
        /// A selector of the form "A B" represents an element B that is an arbitrary descendant of some ancestor element A.
        /// </summary>
        Descendant,
        /// <summary>
        /// '>'
        /// A child combinator describes a childhood relationship between two elements. 
        /// A child combinator is made of the "greater-than sign" (U+003E, >) character and separates two sequences of simple selectors.
        /// </summary>
        Child,
        /// <summary>
        /// '+'
        /// The next-sibling combinator is made of the "plus sign" (U+002B, +) character that separates two sequences of simple selectors. 
        /// The elements represented by the two sequences share the same parent in the document tree and the element represented by 
        /// the first sequence immediately precedes the element represented by the second one.
        /// </summary>
        Sibling_Adjacent,
        /// <summary>
        /// '~'
        /// The subsequent-sibling combinator is made of the "tilde" (U+007E, ~) character that separates two sequences of simple selectors. 
        /// The elements represented by the two sequences share the same parent in the document tree and the element represented 
        /// by the first sequence precedes (not necessarily immediately) the element represented by the second one.
        /// </summary>
        Sibling_Subsequent,
    }
}
