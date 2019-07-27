using System;

namespace CssUI.DOM.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [Obsolete("Use CssUI.MetaEnum")]
    public class DomEnumAttribute : System.Attribute
    {
        [Obsolete("Use CssUI.MetaEnum")]
        public DomEnumAttribute()
        {
        }
    }
}
