using System.Collections.Generic;

namespace CssUI
{
    public class CssPropertySetComparator : IComparer<CssPropertySet>
    {
        // DOCS: https://www.w3.org/TR/CSS22/cascade.html#cascading-order
        public int Compare(CssPropertySet x, CssPropertySet y)
        {

            // When sorting we need to to take into account the "order" in which these rules would have been declared in a stylesheet, this is indicated by the PropertySets ID number with lower ids being older.
            // We also have to take into account:
            //  - Origin of the set (Author, User, UserAgent)
            //  - Selector specificity

            /*
             * Sort according to importance (normal or important) and origin (author, user, or user agent). In ascending order of precedence:
                user agent declarations
                user normal declarations
                author normal declarations
                author important declarations
                user important declarations
             */

             // XXX: We do not take into account the '!important' flag atm as we are sorting not by individual propertys but by entire property blocks containing many properties.
            if (x.Origin != y.Origin)
            {
                if (x.Origin > y.Origin)
                    return 1;// this set is of lesser origin, it belongs behind us
                else
                    return -1;// this set is of greater origin so we go behind it
            }

            //if (x.Selector.Get_Specificity(x.Owner)

            return 0;
        }
    }
}
