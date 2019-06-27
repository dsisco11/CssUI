﻿using System;
using System.Collections.Generic;

namespace CssUI.CSS
{
    /// <summary>
    /// A Complex selector consists of a Compound selector followed by a Combinator
    /// </summary>
    public class CssComplexSelector : CssCompoundSelector
    {// SEE:  https://drafts.csswg.org/selectors-4/#ref-for-typedef-complex-selector-1
        /// <summary>
        /// The combinator following the selector sequence.
        /// </summary>
        public readonly ESelectorCombinator Combinator;

        /// <summary>
        /// Creates a new selector sequence
        /// </summary>
        /// <param name="Chain"></param>
        /// <param name="Combinator">The combinator that comes AFTER the selector sequence.</param>
        public CssComplexSelector(ESelectorCombinator Combinator, CssCompoundSelector Compound) : base(Compound)
        {
            this.Combinator = Combinator;
        }

        public bool Query(LinkedList<cssElement> MatchList, ESelectorMatchingOrder Order)
        {
            switch (Order)
            {
                case ESelectorMatchingOrder.LTR:
                    {// Going from left-to-right means we modify our MatchList with the combinator AFTER we examine it
                        base.Query(MatchList, Order);
                        Apply_Combinator(MatchList, Order);
                    }
                    break;
                case ESelectorMatchingOrder.RTL:
                    {// Going from right-to-left means we modify our MatchList with the combinator BEFORE we examine it
                        Apply_Combinator(MatchList, Order);
                        base.Query(MatchList, Order);
                    }
                    break;
            }

            return true;
        }

        void Apply_Combinator(LinkedList<cssElement> MatchList, ESelectorMatchingOrder Dir)
        {
            // Transform the matchlist using our combinator
            switch (Combinator)
            {
                default:
                    throw new NotImplementedException(string.Concat("[CSS][Selector] Unhandled selector-combinator(", Enum.GetName(typeof(ESelectorCombinator), Combinator), ")!"));
            }
        }
    }

}