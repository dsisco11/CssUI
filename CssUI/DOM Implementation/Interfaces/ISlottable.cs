namespace CssUI.DOM.Nodes
{
    public interface ISlottable : INode
    {
        string Slot_Name { get; set; }
        ISlot assignedSlot { get; set; }
    }
}
