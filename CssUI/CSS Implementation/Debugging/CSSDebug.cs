namespace CssUI
{
    public static class CSSDebug
    {

        public static void Hook_Property_Changes(ICssProperty Property)
        {
            Property.onValueChange += (ECssPropertyStage Stage, ICssProperty Prop) => {
                System.Diagnostics.Debugger.Break();
            };
        }
    }
}
