using System.Collections.Generic;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents a collection of selector filtering items
    /// </summary>
    public class CssSelectorFilterSet : List<ICssSelectorFilter>
    {

        public CssSelectorFilterSet()
        {
        }

        public bool Query(LinkedList<cssElement> MatchList, ESelectorMatchingOrder Dir)
        {
            foreach (ICssSelectorFilter selector in this)
            {
                // We are filtering the match list
                selector.Query(MatchList, Dir);
                // Looks like none of our filters matched the elements
                if (MatchList.Count <= 0)
                    break;
            }

            return true;// there were SOME matches
        }

        /// <summary>
        /// Returns the selectors specificity as defined in the CSS 2.1 specification documentation
        /// </summary>
        public long Get_Specificity(bool IsFromStylesheet)
        {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#specificity
            long A = 0, B = 0, C = 0, D = 0;
            if (IsFromStylesheet) A = 1;

            foreach (ICssSelectorFilter Filter in this)
            {
                foreach (CssSimpleSelector Simple in Filter.Get_Selectors())
                {
                    switch (Simple.Type)
                    {
                        case ECssSimpleSelectorType.IDSelector:
                            B += 1;
                            break;
                        case ECssSimpleSelectorType.AttributeSelector:
                        case ECssSimpleSelectorType.ClassSelector:
                        case ECssSimpleSelectorType.PseudoClassSelector:
                            C += 1;
                            break;
                        case ECssSimpleSelectorType.TypeSelector:
                        case ECssSimpleSelectorType.PseudoElementSelector:
                            D += 1;
                            break;
                    }
                }
            }

            return ((A << 00) | (B << 16) | (C << 32) | (D << 48));
        }
    }

}
