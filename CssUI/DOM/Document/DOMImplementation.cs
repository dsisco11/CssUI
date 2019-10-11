using CssUI.DOM.Exceptions;

namespace CssUI.DOM
{
    public class DOMImplementation
    {/* Docs: https://dom.spec.whatwg.org/#domimplementation */


        public DocumentType createDocumentType(string qualifiedName, string publicId = "", string systemId = "")
        {
            /* If qualifiedName does not match the Name production, an "InvalidCharacterError" DOMException is thrown */
            if (!XMLCommon.Is_Valid_Name(qualifiedName))
                throw new InvalidCharacterError($"qualifiedName '{qualifiedName}' is not in valid XML format!");

            /* ...and if it does not match the QName production, a "NamespaceError" DOMException is thrown. */
            if (!XMLCommon.Is_Valid_QName(qualifiedName))
                throw new NamespaceError();

            return new DocumentType(qualifiedName, publicId, systemId);
        }

        public XMLDocument createDocument(string Namespace, string qualifiedName, DocumentType doctype = null)
        {/* Docs: https://dom.spec.whatwg.org/#dom-domimplementation-createdocument */
            var document = new XMLDocument(Namespace);
            Element element = null;

            if (!string.IsNullOrEmpty(qualifiedName))
            {
                element = DOMCommon.createElementNS(document, qualifiedName, Namespace);
            }

            /* 4) If doctype is non-null, append doctype to document. */
            if (doctype != null)
                document.append(doctype);
            /* 5) If element is non-null, append element to document. */
            if (element != null)
                document.append(element);

            /* 6) document’s origin is context object’s associated document’s origin. */

            return document;
        }

        public Document createHTMLDocument(string title)
        {
            var doc = new HTMLDocument("text/html");
            doc.append(new DocumentType("html") { nodeDocument = doc });
            /* 4) Append the result of creating an element given doc, html, and the HTML namespace, to doc. */
            var html = DOMCommon.createElementNS(doc, "html", DOMCommon.HTMLNamespace);
            doc.append(html);
            /* 5) Append the result of creating an element given doc, head, and the HTML namespace, to the html element created earlier. */
            var head = DOMCommon.createElementNS(doc, "head", DOMCommon.HTMLNamespace);
            html.append(head);
            /* 6) If title is given: */
            if (title != null)
            {
                /* 1) Append the result of creating an element given doc, title, and the HTML namespace, to the head element created earlier. */
                var titleElement = DOMCommon.createElementNS(doc, "title", DOMCommon.HTMLNamespace);
                head.append(titleElement);
                /* 2) Append a new Text node, with its data set to title (which could be the empty string) and its node document set to doc, to the title element created earlier. */
                var textNode = new Text(doc, title);
                titleElement.append(textNode);
            }
            /* 7) Append the result of creating an element given doc, body, and the HTML namespace, to the html element created earlier. */
            var body = DOMCommon.createElementNS(doc, "body", DOMCommon.HTMLNamespace);
            html.append(body);

            return doc;
        }

        /*boolean hasFeature(); // useless; always returns true*/
    }
}
