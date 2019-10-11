using System;
using System.Reflection;
using CssUI.DOM;

namespace CssUI
{
    public class ElementMetadata
    {
        #region Instances
        public static ElementMetadata ElementMeta = new ElementMetadata(string.Empty, typeof(Element));
#if ENABLE_HTML
        public static ElementMetadata UnknownMeta = new ElementMetadata(string.Empty, typeof(HTML.HTMLUnknownElement));
#endif
        #endregion

        #region Static
        static Type[] CtorTypes = new Type[] { typeof(Document), typeof(string), typeof(string), typeof(string) };
        #endregion

        #region Properties
        public readonly string LocalName;
        public readonly Type ElementType;
        public readonly ConstructorInfo ctor;
        #endregion

        #region Constructors
        public ElementMetadata(string localName, Type elementType)
        {
            LocalName = localName;
            ElementType = elementType;
            ctor = elementType.GetConstructor(CtorTypes);
        }
        #endregion


    }
}
