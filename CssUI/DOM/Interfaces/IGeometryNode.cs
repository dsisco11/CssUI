using CssUI.DOM.Events;
using CssUI.DOM.Geometry;
using CssUI.DOM.Internal;
using System.Collections.Generic;

namespace CssUI.DOM.Interfaces
{
    public interface IGeometryNode : IEventTarget
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#the-geometryutils-interface */
        IEnumerable<DOMQuad> getBoxQuads(BoxQuadOptions options);
        DOMQuad convertQuadFromNode(DOMQuadInit quad, IGeometryNode from, ConvertCoordinateOptions options);
        DOMQuad convertRectFromNode(DOMRectReadOnly rect, IGeometryNode from, ConvertCoordinateOptions options);
        DOMPoint convertPointFromNode(DOMPointInit point, IGeometryNode from, ConvertCoordinateOptions options); // XXX z,w turns into 0
    }
}
