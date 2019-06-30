using CssUI.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{
    public class MultiStringProperty : CssMultiValueProperty
    {
        #region Value Overrides
        public new IEnumerable<string> Actual
        {
            get {
                return base.Actual.Select<string>((CssValue v) => { return (string)v.Value; });
            }
        }
        #endregion

        #region Constructors
        public MultiStringProperty(string CssName, bool Locked, WeakReference<CssPropertySet> Source, cssElement Owner) : base(CssName, Locked, Source, Owner)
        {
        }
        #endregion

        #region Setters
        public void Set(string str)
        {
            CssValueList newValueList = new CssValueList(CssValue.From_String(str));
            if (Assigned != newValueList)
            {
                Assigned = newValueList;
            }
        }

        public void Set(IEnumerable<string> strList)
        {
            CssValueList newValueList = new CssValueList(strList.Select(x => CssValue.From_String(x)));
            if (Assigned != newValueList)
            {
                Assigned = newValueList;
            }
        }
        #endregion
    }
}
