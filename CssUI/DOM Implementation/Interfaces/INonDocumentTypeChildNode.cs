namespace CssUI.DOM
{
    public interface INonDocumentTypeChildNode
    {
        Element previousElementSibling { get; }
        Element nextElementSibling { get; }
    }
}
