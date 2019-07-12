namespace CssUI.DOM
{
    public class XMLDocument : Document
    {/* Docs: https://dom.spec.whatwg.org/#xmldocument */


        public XMLDocument(string Namespace) : base(DocumentType.XML, DOMCommon.Lookup_Content_Type_String(Namespace))
        {
        }
    }
}
