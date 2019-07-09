using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
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
             * So our selector implementation is a little different from the W3C specifications in how it STRUCTURES its matching process, but the logic is the same. 
             */
            /* Right-to-Left matching enforced here */
            /* We run through our list of relative selectors and match them against our element, if ANY fail then the whole selector fails to match. */
            LinkedList<Element> matchList = new LinkedList<Element>(new Element[] { element });
            for (int i=Count-1; i>=0; i--)
            {
                if (!this[i].Match(matchList, out LinkedList<Element> outMatchList, scopeElements))
                    return false;

                matchList = outMatchList;
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

            return ((A << 00) | (B << 16) | (C << 32));
        }
        #endregion
    }

}
