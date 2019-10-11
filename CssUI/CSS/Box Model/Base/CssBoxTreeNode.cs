using CssUI.NodeTree;

namespace CssUI.CSS.BoxTree
{
    /// <summary>
    /// Simplest form of an object that participates in the CSS box-model.
    /// Has a Position, Size, Start, and End.
    /// All objects participating in the box-model must inherit from this.
    /// </summary>
    public class CssBoxTreeNode : TreeNode
    {
        #region Properties
        /// <summary>
        /// Represents the object responsible for rendering this boxs' objects/contents.
        /// </summary>
        internal object RenderObject = null;
        /// <summary>
        /// The parent object from which this one descends
        /// </summary>
        public CssBoxTreeNode parentBox => parentNode as CssBoxTreeNode;
        /// <summary>
        /// Layout position
        /// </summary>
        public Point2f Position { get; set; }
        /// <summary>
        /// Layout dimensions
        /// </summary>
        public virtual ReadOnlyRect2f Size { get; set; }

        #region TreeNode Overrides
        /// <inheritdoc/>
        public new CssBoxTreeNode parentNode { get => (CssBoxTreeNode)base.parentNode; set => base.parentNode = value; }
        /// <inheritdoc/>
        public new CssBoxTreeNode firstChild => (CssBoxTreeNode)base.firstChild;
        /// <inheritdoc/>
        public new CssBoxTreeNode lastChild => (CssBoxTreeNode)base.lastChild;
        /// <inheritdoc/>
        public new CssBoxTreeNode nextSibling => (CssBoxTreeNode)base.nextSibling;
        /// <inheritdoc/>
        public new CssBoxTreeNode previousSibling => (CssBoxTreeNode)base.previousSibling;
        #endregion
        #endregion

        /* XXX: Create a class which holds a value for each CSS property but which links them by reference to a parent objects values unless overriden */

        public CssBoxTreeNode(CssBoxTreeNode parent) : base(parent)
        {
            Position = Point2f.Zero;
            Size = Rect2f.Zero;
        }

        /// <summary>
        /// Unlinks a series or chain of nodes starting at this one and traversing upwards before finally ending at the given <paramref name="TerminationPoint"/>
        /// </summary>
        /// <returns>Root of the unlinked chain</returns>
        public ITreeNode Unlink(TreeNode TerminationPoint)
        {
            var Junction = Tree.Get_Junction(this, TerminationPoint);
            index = Junction.index;
            ((TreeNode)Junction.parentNode).Remove(Junction);
            Junction.parentNode = null;

            // Walk the chain to unlink all nodes so they are dereferenced
            ITreeNode n = this;
            while (!(n is null) && !(n.parentNode is null))
            {
                System.Diagnostics.Debug.Assert(n.childNodes.Count <= 1);
                var last = n;
                n = n.parentNode;
                last.parentNode = null;
            }

            return Junction;
        }
    }
}
