namespace CssUI.CSS.Internal
{
    /// <summary>
    /// A function that transforms a percentage into an absolute value
    /// </summary>
    /// <param name="Property">The property being resolved</param>
    /// <param name="Percent">Decimal in the range 0.0 - 1.0</param>
    /// <returns></returns>
    public delegate CssValue CssPercentageResolver(ICssProperty Property, double Percent);

    public static class CssPercentageResolvers
    {
        public static CssValue Containing_Block_Logical_Width(ICssProperty Property, double Percent)
        {
            if (!Property.Owner.Box.Containing_Box_Explicit_Width)
            {
                return CssValue.Zero;
            }
            else
            {
                var resolved = Percent * CssCommon.Get_Logical_Width(Property.Owner.Style.WritingMode, Property.Owner.Box.Containing_Box);
                return CssValue.From(resolved, ECssUnit.PX);
            }
        }

        public static CssValue Containing_Block_Logical_Height(ICssProperty Property, double Percent)
        {
            if (!Property.Owner.Box.Containing_Box_Explicit_Height)
            {
                return CssValue.Zero;
            }
            else
            {
                var resolved = Percent * CssCommon.Get_Logical_Height(Property.Owner.Style.WritingMode, Property.Owner.Box.Containing_Box);
                return CssValue.From(resolved, ECssUnit.PX);
            }
        }
    }
}
