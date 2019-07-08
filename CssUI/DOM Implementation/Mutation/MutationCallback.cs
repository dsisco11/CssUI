using System.Collections.Generic;

namespace CssUI.DOM.Mutation
{
    public delegate void MutationCallback(IEnumerable<MutationRecord> mutations, MutationObserver observer);
}
