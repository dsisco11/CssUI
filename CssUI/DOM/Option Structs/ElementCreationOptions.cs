namespace CssUI.DOM
{
    public class ElementCreationOptions
    {
        /// <summary>
        /// Specifies a custom element class name
        /// This is called the "is" attribute by the DOM specs.
        /// </summary>
        public readonly string CustomClassName;

        public ElementCreationOptions(string customClassName)
        {
            CustomClassName = customClassName;
        }
    }
}
