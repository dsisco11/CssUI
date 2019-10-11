using System;
using System.Collections.Generic;
using System.Linq;
using CssUI.DOM.Nodes;
using CssUI.Internal;

namespace CssUI.CSS
{
    public class MultiStringProperty : CssMultiValueProperty
    {
        #region Value Overrides
        /// <inheritdoc/>
        public new IEnumerable<string> Actual => base.Actual.Select(v => v.AsString());
        #endregion

        #region Constructors
        public MultiStringProperty(AtomicName<ECssPropertyID> CssName, ICssElement Owner, WeakReference<CssComputedStyle> Source, bool Locked)
            : base(CssName, Locked, Source, Owner)
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
