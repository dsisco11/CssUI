using CssUI.DOM.Geometry;
using System.Collections.Generic;

namespace CssUI.DOM.Interfaces
{
    public interface IGeometryNode
    {
        IEnumerable<DOMQuad> getBoxQuads(BoxQuadOptions options);
        DOMQuad convertQuadFromNode(DOMQuadInit quad, IGeometryNode from, ConvertCoordinateOptions options);
        DOMQuad convertRectFromNode(DOMRectReadOnly rect, IGeometryNode from, ConvertCoordinateOptions options);
        DOMPoint convertPointFromNode(DOMPointInit point, IGeometryNode from, ConvertCoordinateOptions options); // XXX z,w turns into 0
    }
}
