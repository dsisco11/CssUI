using System.Collections.Generic;
using CssUI.DOM;

namespace CssUI.CSS;

public class CssPropertyComparator : IComparer<ICssProperty>
{
    public readonly static CssPropertyComparator Instance = new CssPropertyComparator();

    // DOCS: https://www.w3.org/TR/CSS22/cascade.html#cascading-order
    public int Compare(ICssProperty? x, ICssProperty? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        // XXX: We do not take into account the '!important' flag atm
        if (x.Source!.Origin != y.Source!.Origin)
        {
            if (x.Source.Origin > y.Source.Origin)
                return 1;// this set is of lesser origin, it belongs behind us
            else
                return -1;// this set is of greater origin, so we go behind it
        }

        long xSpec = x.Selector?.Get_Specificity((x.Owner as Element)!) ?? 0;
        long ySpec = y.Selector?.Get_Specificity((y.Owner as Element)!) ?? 0;
        if (xSpec != ySpec)
        {
            if (xSpec > ySpec)
                return 1;// this set isnt specified as specifically as us, so we go ahead of it
            else
                return -1;// this set ismore specific than us, so we go behind it
        }

        ulong xAge = x.Source.ID;
        ulong yAge = y.Source.ID;
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

