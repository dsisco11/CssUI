namespace CssUI.DOM
{
    /// <summary>
    /// Skips any nodes that are not <see cref="HTMLDataListElement"/>
    /// </summary>
    public class FilterDatalistElement : FilterElementType<HTMLDataListElement>
    {
        public static readonly NodeFilter Instance = new FilterDatalistElement();

        public FilterDatalistElement()
        {
        }
    }
}
