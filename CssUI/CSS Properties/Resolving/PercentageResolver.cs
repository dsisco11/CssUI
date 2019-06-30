namespace CssUI.Internal
{
    /// <summary>
    /// A function that transforms a percentage value int an absolute one for a property 
    /// </summary>
    /// <param name="E"></param>
    /// <param name="Percent"></param>
    /// <returns></returns>
    public delegate double PercentageResolver(cssElement E, double Percent);
}
