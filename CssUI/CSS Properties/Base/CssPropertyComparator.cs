using System.Collections.Generic;

namespace CssUI.CSS
{
    public class CssPropertyComparator : IComparer<ICssProperty>
    {
        // DOCS: https://www.w3.org/TR/CSS22/cascade.html#cascading-order
        public int Compare(ICssProperty x, ICssProperty y)
        {
            // XXX: We do not take into account the '!important' flag atm
            if (x.Get_Source().Origin != y.Get_Source().Origin)
            {
                if (x.Get_Source().Origin > y.Get_Source().Origin)
                    return 1;// this set is of lesser origin, it belongs behind us
                else
                    return -1;// this set is of greater origin, so we go behind it
            }

            long xSpec = x.Selector.Get_Specificity(x.Owner);
            long ySpec = y.Selector.Get_Specificity(y.Owner);
            if (xSpec != ySpec)
            {
                if (xSpec > ySpec)
                    return 1;// this set isnt specified as specifically as us, so we go ahead of it
                else
                    return -1;// this set ismore specific than us, so we go behind it
            }

            ulong xAge = x.Get_Source().ID;
            ulong yAge = y.Get_Source().ID;
            if (xAge != yAge)
            {
                if (xAge > yAge)
                    return 1;// this set is of lesser age, it belongs behind us
                else
                    return -1;// this set is of greater age, so we go behind it
            }

            // These things are exactly the same, return 0
            return 0;
        }
    }
}
