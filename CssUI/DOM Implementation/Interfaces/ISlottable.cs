namespace CssUI.DOM.Nodes
{
    public interface ISlottable : INode
    {
        string Name { get; set; }
        ISlot assignedSlot { get; set; }
    }
}
