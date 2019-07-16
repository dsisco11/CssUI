using CssUI.DOM;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.Fonts
{
    /// <summary>
    /// Provides metrics data for text & font objects
    /// </summary>
    public static class FontMetrics
    {

        public static DOMRect getTextRect(Node node, int start, int count)
        {
            if (node.nodeType != DOM.Enums.ENodeType.TEXT_NODE)
                return new DOMRect(0, 0, 0, 0);

            /* XXX: FontMetrics implement thisssss */

            throw new NotImplementedException();
        }

    }
}
