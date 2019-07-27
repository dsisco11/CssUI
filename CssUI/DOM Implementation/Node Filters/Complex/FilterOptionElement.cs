namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only <see cref="HTMLOptionElement"/>
    /// </summary>
    public class FilterOptionElement : FilterElementType<HTMLOptionElement>
    {
        public static readonly NodeFilter Instance = new FilterOptionElement();

        public FilterOptionElement()
        {
        }
    }
}
