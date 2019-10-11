namespace CssUI.NodeTree
{
    public abstract class NodeTreeFilter
    {
        public abstract EFilterResult acceptNode(ITreeNode node);
    }
}
