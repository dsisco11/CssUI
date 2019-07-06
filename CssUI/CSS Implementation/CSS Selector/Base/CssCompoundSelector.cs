using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{

    /// <summary>
    /// A Compound selector is one that consists of multiple simple selectors
    /// In addition a Compound selector can contain at most ONE type-selector and if it does, that must be the first selector within it.
    /// </summary>
    public class CssCompoundSelector : ICssSelectorFilter
    {// SEE:  https://drafts.csswg.org/selectors-4/#typedef-compound-selector

        public readonly List<CssSimpleSelector> Selectors = null;
        #region Constructors
        public CssCompoundSelector()
        {
            this.Selectors = new List<CssSimpleSelector>();
        }

        public CssCompoundSelector(IEnumerable<CssSimpleSelector> Collection)
        {
            this.Selectors = new List<CssSimpleSelector>(Collection);
            CssSimpleSelector ts = Selectors.FirstOrDefault(o => o is CssTypeSelector);
            if (ts != null && ts != Selectors[0] || Selectors.Count(o => o is CssTypeSelector) > 1)
                throw new CssSyntaxErrorException("Compound selectors can only contain a single type-selector and it MUST be the first selector in the list!");
        }

        public CssCompoundSelector(CssCompoundSelector Compound) : this(Compound.Selectors)
        {
        }
        #endregion

        public List<CssSimpleSelector> Get_Selectors() { return Selectors; }
        public bool Query(LinkedList<cssElement> MatchList, ESelectorMatchingOrder Order)
        {
            if (MatchList.Count <= 0)
                return false;
            // Filter the matchlist with our simple selectors first
            LinkedListNode<cssElement> node = MatchList.First;
            do
            {
                bool bad = false;// if True then we remove the node from our list
                switch (Order)
                {
                    case ESelectorMatchingOrder.LTR:
                        {
                            for (int i = 0; i < Selectors.Count; i++)// progressing forwards
                            {
                                CssSimpleSelector Selector = Selectors[i];
                                if (!Selectors[i].Matches(node.Value))
                                {
                                    bad = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case ESelectorMatchingOrder.RTL:
                        {
                            for (int i = Selectors.Count - 1; i >= 0; i--)// progressing backwards
                            {
                                CssSimpleSelector Selector = Selectors[i];
                                if (!Selectors[i].Matches(node.Value))
                                {
                                    bad = true;
                                    break;
                                }
                            }
                        }
                        break;
                }

                if (bad)
                {
                    var curr = node;// Track current node just incase next is null or something
                    node = node.Next;// Progress
                    MatchList.Remove(curr);// Remove this element
                }
                else
                {
                    node = node.Next;
                }
            }
            while (node != null);

            return true;
        }
    }
}
