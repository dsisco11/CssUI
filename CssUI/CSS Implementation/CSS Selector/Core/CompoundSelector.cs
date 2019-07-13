using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    /// <summary>
    /// A Compound selector is one that consists of multiple simple selectors
    /// In addition a Compound selector can contain at most ONE type-selector and if it does, that must be the first selector within it.
    /// </summary>
    public class CompoundSelector : List<SimpleSelector>
    {/* Docs: https://drafts.csswg.org/selectors-4/#typedef-compound-selector */

        #region Constructors
        public CompoundSelector()
        {
        }

        public CompoundSelector(IEnumerable<SimpleSelector> Collection) : base(Collection)
        {
            SimpleSelector ts = this.FirstOrDefault(o => o is TypeSelector);
            if (ts != null && ts != this[0] || this.Count(o => o is TypeSelector) > 1)
                throw new CssSyntaxErrorException("Compound selectors can only contain a single type-selector and it MUST be the first selector in the list!");
        }

        public CompoundSelector(CompoundSelector Compound) : this(Compound.ToArray())
        {
        }
        #endregion

        #region Matching
        /// <summary>
        /// Performs simple selector matching against all elements in <paramref name="MatchList"/>.
        /// Modifies the list, removing any non-matching elements
        /// </summary>
        /// <param name="MatchList">List of elements to match against</param>
        /// <param name="Order">The matching direction</param>
        /// <returns>True if any elements were a match</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool Match(IEnumerable<Element> MatchList, out LinkedList<Element> outList, ESelectorMatchingOrder Order, params Node[] scopeElements)
        {
            if (MatchList.Count() <= 0)
            {
                outList = (LinkedList<Element>)MatchList;
                return false;
            }

            var newList = new LinkedList<Element>();
            foreach (Element element in MatchList)
            {
                bool fullMatch = true;
                switch (Order)
                {
                    case ESelectorMatchingOrder.LTR:
                        {
                            for (int i = 0; i < Count; i++)// progressing forwards
                            {
                                SimpleSelector Selector = this[i];
                                if (!Selector.Matches(element, scopeElements))
                                {
                                    fullMatch = false;
                                    break;
                                }
                            }
                        }
                        break;
                    case ESelectorMatchingOrder.RTL:
                        {
                            for (int i = Count - 1; i >= 0; i--)// progressing backwards
                            {
                                SimpleSelector Selector = this[i];
                                if (!Selector.Matches(element, scopeElements))
                                {
                                    fullMatch = false;
                                    break;
                                }
                            }
                        }
                        break;
                }

                if (fullMatch)
                {
                    newList.AddLast(element);
                }
            }

            outList = newList;
            return (newList.Count > 0);
        }

        #endregion
    }
}
