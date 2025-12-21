using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

/// <summary>
/// A Complex selector holds a set of one or more relative selectors.
/// It is essentially the encapsulation of all content that defines an individual "selector"
/// </summary>
public class ComplexSelector : List<RelativeSelector>
{/* Docs: https://drafts.csswg.org/selectors-4/#typedef-complex-selector */

    #region Constructors
    public ComplexSelector() : base()
    {
    }

    public ComplexSelector(IEnumerable<RelativeSelector> Collection) : base(Collection)
    {
    }
    #endregion

    #region Matching

    /// <summary>
    /// Performs matching on an element.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Match(Element element, params Node[] scopeElements)
    {/* Docs: https://drafts.csswg.org/selectors-4/#match-a-complex-selector-against-an-element */
        /*
         * Right-to-Left matching: start from the rightmost selector (the subject) and work backwards.
         * The combinator in selector[i] indicates how to find candidates for selector[i] from elements matching selector[i+1].
         */
        LinkedList<Element> matchList = new LinkedList<Element>(new Element[] { element });

        for (int i = Count - 1; i >= 0; i--)
        {
            // Match the compound selector against current candidates
            this[i].MatchCompound(matchList, out LinkedList<Element> matches, scopeElements);

            if (matches.Count == 0)
                return false;

            // If this isn't the first selector (leftmost), apply the combinator to get next candidates
            if (i > 0)
            {
                // Use the combinator from the selector to the LEFT (this[i-1]) to find candidates
                var matchSet = new HashSet<Element>();
                foreach (Element matched in matches)
                {
                    var candidates = this[i - 1].Apply_Combinator(matched, ESelectorMatchingOrder.RTL);
                    matchSet.UnionWith(candidates);
                }
                matchList = new LinkedList<Element>(matchSet);
            }
            else
            {
                matchList = matches;
            }
        }

        return true;
    }
    #endregion

    #region Specificity

    /// <summary>
    /// Returns the selectors specificity as defined in the CSS 2.1 specification documentation
    /// </summary>
    public long Get_Specificity()
    {/* Docs: https://www.w3.org/TR/selectors-3/#specificity */
        long A = 0, B = 0, C = 0;

        foreach (RelativeSelector Relative in this)
        {
            foreach (SimpleSelector Simple in Relative)
            {
                switch (Simple.Type)
                {
                    case ESimpleSelectorType.IDSelector:
                        A++;
                        break;
                    case ESimpleSelectorType.ClassSelector:
                    case ESimpleSelectorType.AttributeSelector:
                    case ESimpleSelectorType.PseudoClassSelector:
                        B++;
                        break;
                    case ESimpleSelectorType.TypeSelector:
                    case ESimpleSelectorType.PseudoElementSelector:
                        C++;
                        break;
                }
            }
        }

        // Specificity: A is most significant (IDs), B is middle (classes), C is least (types)
        return ((A << 32) | (B << 16) | (C << 00));
    }
    #endregion
}

