using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting
{
    public class BlockFormattingContext : IFormattingContext
    {
        public void Flow(CssBoxTreeNode Node)
        {
            if (Node is null)
            {
                throw new System.ArgumentNullException(nameof(Node));
            }
            Contract.EndContractBlock();

            CssBoxTreeNode Current = Node.firstChild;
            while (Current is object)
            {
                if (Current.previousSibling is null)
                {
                    Current.Position = Point2f.Zero; 
                }
                else
                {
                    var prev = Current.previousSibling;
                    var prev_pos = prev.Position;
                    var prev_size = prev.Size;
                    Current.Position = new Point2f(prev_pos.X + prev_size.Width, prev_pos.Y + prev_size.Height);
                }

                Current = Current.nextSibling;
            }
        }
    }
}
