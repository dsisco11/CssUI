using System;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;

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

#if DISABLE_FONT_SYSTEM
            return new DOMRect(start, 1, start+count, 0);
#else
            /* XXX: FontMetrics implement thisssss */

            throw new NotImplementedException();
#endif
        }

    }
}
