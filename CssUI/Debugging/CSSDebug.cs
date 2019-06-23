using CssUI;

namespace CssUI
{
    public static class CSSDebug
    {

        public static void Track_Property_Changes(IStyleProperty Property)
        {
            Property.onChanged += (IStyleProperty Prop) => {
                System.Diagnostics.Debugger.Break();
            };
        }
    }
}
