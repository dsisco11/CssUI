using CssUI;

namespace CssUI
{
    public static class CSSDebug
    {

        public static void Track_Property_Changes(ICssProperty Property)
        {
            Property.onChanged += (ICssProperty Prop) => {
                System.Diagnostics.Debugger.Break();
            };
        }
    }
}
