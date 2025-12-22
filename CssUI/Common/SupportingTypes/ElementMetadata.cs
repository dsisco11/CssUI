using System;
using System.Reflection;
using CssUI.DOM;

namespace CssUI;

public class ElementMetadata
{
    #region Static
    // Possible constructor signatures for HTML elements (tried in order)
    static Type[][] CtorSignatures = new Type[][]
    {
        new Type[] { typeof(Document), typeof(string), typeof(string), typeof(string) },  // Element(doc, localName, prefix, namespace)
        new Type[] { typeof(Document), typeof(string) },                                   // HTMLElement(doc, localName)
        new Type[] { typeof(Document) }                                                    // HTMLBodyElement(doc)
    };
    #endregion

    #region Instances
    public static ElementMetadata ElementMeta = new ElementMetadata(string.Empty, typeof(Element));
#if ENABLE_HTML
    public static ElementMetadata UnknownMeta = new ElementMetadata(string.Empty, typeof(HTML.HTMLUnknownElement));
#endif
    #endregion

    #region Properties
    public readonly string LocalName;
    public readonly Type ElementType;
    public readonly ConstructorInfo? ctor;
    public readonly int CtorParameterCount;
    #endregion

    #region Constructors
    public ElementMetadata(string localName, Type elementType)
    {
        LocalName = localName;
        ElementType = elementType;

        // Try each constructor signature in order
        foreach (var ctorTypes in CtorSignatures)
        {
            ctor = elementType.GetConstructor(ctorTypes);
            if (ctor != null)
            {
                CtorParameterCount = ctorTypes.Length;
                break;
            }
        }
    }
    #endregion
}

