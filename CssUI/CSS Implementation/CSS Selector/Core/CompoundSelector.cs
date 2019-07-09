using CssUI.DOM;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS.Selectors
{

    /// <summary>
    /// A Compound selector is one that consists of multiple simple selectors
    /// In addition a Compound selector can contain at most ONE type-selector and if it does, that must be the first selector within it.
    /// </summary>
    public class CompoundSelector : ISelectorFilter
    {// SEE:  https://drafts.csswg.org/selectors-4/#typedef-compound-selector

        public readonly List<SimpleSelector> Selectors = null;
        #region Constructors
        public CompoundSelector()
        {
            this.Selectors = new List<SimpleSelector>();
        }

        public CompoundSelector(IEnumerable<SimpleSelector> Collection)
        {
            this.Selectors = new List<SimpleSelector>(Collection);
            SimpleSelector ts = Selectors.FirstOrDefault(o => o is CssTypeSelector);
            if (ts != null && ts != Selectors[0] || Selectors.Count(o => o is CssTypeSelector) > 1)
                throw new CssSyntaxErrorException("Compound selectors can only contain a single type-selector and it MUST be the first selector in the list!");
        }

        public CompoundSelector(CompoundSelector Compound) : this(Compound.Selectors)
        {
        }
        #endregion

        public List<SimpleSelector> Get_Selectors() { return Selectors; }
        public bool Query(LinkedList<Element> MatchList, ESelectorMatchingOrder Order)
        {
            if (MatchList.Count <= 0)
                return false;
            // Filter the matchlist with our simple selectors first
            LinkedListNode<Element> node = MatchList.First;
            do
            {
                bool bad = false;// if True then we remove the node from our list
                switch (Order)
                {
                    case ESelectorMatchingOrder.LTR:
                        {
                            for (int i = 0; i < Selectors.Count; i++)// progressing forwards
                            {
                                SimpleSelector Selector = Selectors[i];
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
                                SimpleSelector Selector = Selectors[i];
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
