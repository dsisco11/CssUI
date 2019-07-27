using System;

namespace CssUI.CSS.Internal
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [Obsolete("Use CssUI.MetaEnum")]
    public class CssEnumAttribute : System.Attribute
    {
        [Obsolete("Use CssUI.MetaEnum")]
        public CssEnumAttribute()
        {
        }
    }
}
