namespace CssUI.DOM.Nodes
{
    public interface ISlottable : INode
    {
#if ENABLE_HTML
        string Slot_Name { get; set; }
        ISlot assignedSlot { get; set; }
#endif
    }
}
