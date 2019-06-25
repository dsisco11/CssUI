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
        public IList<long> Get_Specificity(cssElement E, bool IsFromStylesheet)
        {
            var retVal = new List<long>();

            var tmpList = new LinkedList<cssElement>();
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

        public bool Query(cssElement E)
        {
            LinkedList<cssElement> List = new LinkedList<cssElement>();
            List.AddFirst(E);

            List<cssElement> res = Query(List, ESelectorMatchingOrder.LTR);

            return res.Contains(E);
        }

        public List<cssElement> Query(LinkedList<cssElement> MatchList, ESelectorMatchingOrder Dir)
        {
            if (this.Count == 1)// Most selectors will just have a single instance
            {
                this[0].Query(MatchList, Dir);
                return new List<cssElement>(MatchList);
            }
            else if (this.Count > 1)
            {
                // If we contain more than a single filter set then the list of matching items we return becomes a collection of any items that were matches by any of our filter sets
                HashSet<cssElement> retVal = new HashSet<cssElement>();
                foreach (CssSelectorFilterSet FilterSet in this)
                {
                    // Create a clone of our initial list that this filter set can alter
                    var tmpMatchList = new LinkedList<cssElement>(MatchList);
                    if (!FilterSet.Query(tmpMatchList, Dir)) continue;
                    foreach (cssElement E in tmpMatchList)
                    {
                        retVal.Add(E);
                    }
                }

                return new List<cssElement>(retVal);
            }

            return null;
        }
    }

}
