using CssUI.CSS.Enums;

namespace CssUI.CSS
{
    public static class CSSDebug
    {

        public static void Hook_Property_Changes(ICssProperty Property)
        {
            Property.onValueChange += (Stage, Prop) =>
            {
                System.Diagnostics.Debugger.Break();
            };
        }
    }
}
