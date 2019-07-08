using CssUI.DOM;
using System.Collections.Generic;

namespace CssUI.CSS
{

    /// <summary>
    /// 
    /// </summary>
    public class CssSelectorList : List<CssSelectorFilterSet>
    {
        /// <summary>
        /// Returns a list of all selector specificitys
        /// </summary>
        /// <returns></returns>
        public IList<long> Get_Specificity(Element E, bool IsFromStylesheet)
        {
            var retVal = new List<long>();

            var tmpList = new LinkedList<Element>();
            tmpList.AddFirst(E);

            foreach (CssSelectorFilterSet FilterSet in this)
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
        public bool Query(Element E)
        {
            LinkedList<Element> List = new LinkedList<Element>();
            List.AddFirst(E);

            List<Element> res = Query(List, ESelectorMatchingOrder.LTR);

            return res.Contains(E);
        }

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
                foreach (CssSelectorFilterSet FilterSet in this)
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
