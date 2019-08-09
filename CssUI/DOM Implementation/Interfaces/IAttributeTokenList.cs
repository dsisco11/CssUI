using System;
using System.Collections;

namespace CssUI.DOM
{
    public interface IAttributeTokenList : IEnumerable
    {
        void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, ReadOnlyMemory<char> Namespace);
    }
}
