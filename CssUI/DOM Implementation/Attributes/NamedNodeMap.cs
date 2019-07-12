using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System.Linq;

namespace CssUI.DOM
{
    public class NamedNodeMap
    {/* Docs: https://dom.spec.whatwg.org/#namednodemap */
        #region Properties
        public int length { get => ownerElement.AttributeList.Count; }
        public Element ownerElement { get; private set; }
        #endregion

        #region Constructor
        public NamedNodeMap(Element Owner)
        {
            this.ownerElement = Owner;
        }
        #endregion

        public Attr this[int index]
        {
            get
            {
                if (index >= ownerElement.AttributeList.Count) return null;
                return ownerElement.AttributeList[index];
            }
            set
            {
                if (index >= ownerElement.AttributeList.Count) return;
                Attr oldAttr = ownerElement.AttributeList[index];
                if (ReferenceEquals(oldAttr, value)) return;
                ownerElement.AttributeList[index] = value;
            }
        }

        /*public Attr this[string qualifiedName]
        {
            get => getNamedItem(qualifiedName);
            set
            {
                find_attribute(qualifiedName, out Attr outAttr, out int outIndex);

                Attr oldAttr = ownerElement.AttributeList[index];
                if (ReferenceEquals(oldAttr, value)) return;
                ownerElement.AttributeList[index] = value;
            }
        }*/


        public Attr item(int index)
        {
            if (index >= ownerElement.AttributeList.Count) return null;

            return ownerElement.AttributeList[index];
        }

        public Attr getNamedItem(string qualifiedName)
        {
            /* To get an attribute by name given a qualifiedName and element element, run these steps: */
            /* 1) If element is in the HTML namespace and its node document is an HTML document, then set qualifiedName to qualifiedName in ASCII lowercase. */
            qualifiedName = qualifiedName.ToLowerInvariant();
            /* 2) Return the first attribute in element’s attribute list whose qualified name is qualifiedName, and null otherwise. */
            return ownerElement.AttributeList.FirstOrDefault(a => 0==string.Compare(a.Name, qualifiedName));
        }

        public Attr setNamedItem(Attr attr)
        {
            /* To set an attribute given an attr and element, run these steps: */
            /* 1) If attr’s element is neither null nor element, throw an "InUseAttributeError" DOMException. */
            if (ReferenceEquals(ownerElement, null)) throw new InUseAttributeError("Element is null!");
            /* 2) Let oldAttr be the result of getting an attribute given attr’s namespace, attr’s local name, and element. */
            this.ownerElement.find_attribute(attr.localName, attr.namespaceURI, out Attr oldAttr);
            /* 3) If oldAttr is attr, return attr. */
            if (ReferenceEquals(oldAttr, attr)) return attr;
            /* 4) If oldAttr is non-null, replace it by attr in element. */
            if (!ReferenceEquals(oldAttr, null))
            {
                ownerElement.AttributeList[oldAttr.Name.ToLowerInvariant()] = attr;
            }
            /* 5) Otherwise, append attr to element. */
            else
            {
                ownerElement.AttributeList.Add(attr.Name.ToLowerInvariant(), attr);
            }

            return oldAttr;
        }

        public Attr removeNamedItem(string qualifiedName)
        {
            /* The removeNamedItem(qualifiedName) method, when invoked, must run these steps: */
            /* 1) Let attr be the result of removing an attribute given qualifiedName and element. */

            /* To remove an attribute by name given a qualifiedName and element element, run these steps: */
            /* 1) Let attr be the result of getting an attribute given qualifiedName and element. */
            this.ownerElement.find_attribute(qualifiedName, out Attr attr);
                /* 2) If attr is non-null, remove it from element. */
                if (!ReferenceEquals(attr, null))
                {
                    ownerElement.AttributeList.Remove(attr.Name.ToLowerInvariant());
                }
            /* 2) If attr is null, then throw a "NotFoundError" DOMException. */
            if (ReferenceEquals(attr, null))
            {
                throw new NotFoundError($"Cannot find attribute \"{qualifiedName}\"");
            }
            /* 3) Return attr. */
            return attr;
        }
    }
}
