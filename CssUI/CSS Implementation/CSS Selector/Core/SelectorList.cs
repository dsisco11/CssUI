using CssUI.DOM;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{

    /// <summary>
    /// 
    /// </summary>
    public class SelectorList : List<SelectorFilterSet>
    {
        /// <summary>
        /// Returns a list of all selector specificitys
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IList<long> Get_Specificity(Element E, bool IsFromStylesheet)
        {
            var retVal = new List<long>();

            var tmpList = new LinkedList<Element>();
            tmpList.AddFirst(E);

            foreach (SelectorFilterSet FilterSet in this)
            {
                if(FilterSet.Query(tmpList, ESelectorMatchingOrder.LTR))
                {
                    retVal.Add( FilterSet.Get_Specificity(IsFromStylesheet) );
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks if the given element matches this selector
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Query(Element E, ESelectorMatchingOrder Dir = ESelectorMatchingOrder.RTL)
        {
            LinkedList<Element> List = new LinkedList<Element>();
            List.AddFirst(E);

            List<Element> res = Query(List, Dir);

            return res.Contains(E);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Element> Query(LinkedList<Element> MatchList, ESelectorMatchingOrder Dir)
        {
            if (this.Count == 1)// Most selectors will just have a single instance
            {
                this[0].Query(MatchList, Dir);
                return new List<Element>(MatchList);
            }
            else if (this.Count > 1)
            {
                // If we contain more than a single filter set then the list of matching items we return becomes a collection of any items that were matches by any of our filter sets
                HashSet<Element> retVal = new HashSet<Element>();
                foreach (SelectorFilterSet FilterSet in this)
                {
                    // Create a clone of our initial list that this filter set can alter
                    var tmpMatchList = new LinkedList<Element>(MatchList);
                    if (!FilterSet.Query(tmpMatchList, Dir)) continue;
                    foreach (Element E in tmpMatchList)
                    {
                        retVal.Add(E);
                    }
                }

                return new List<Element>(retVal);
            }

            return null;
        }
    }

}
