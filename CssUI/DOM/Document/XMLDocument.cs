namespace CssUI.DOM;

public class XMLDocument : Document
{/* Docs: https://dom.spec.whatwg.org/#xmldocument */

    /// <summary>
    /// Creates an XMLDocument with a default XML doctype.
    /// </summary>
    public XMLDocument(string Namespace) : base(DocumentType.XML, DOMCommon.Lookup_Content_Type_String(Namespace))
    {
    }

    /// <summary>
    /// Creates an XMLDocument without a doctype. Used by DOMImplementation.createDocument.
    /// </summary>
    internal XMLDocument(string Namespace, bool withoutDoctype) : base(DOMCommon.Lookup_Content_Type_String(Namespace))
    {
    }
}

