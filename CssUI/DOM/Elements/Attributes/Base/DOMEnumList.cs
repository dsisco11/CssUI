using System;

namespace CssUI.DOM;

public class DOMEnumList<EnumType> : AttributeTokenList<EnumType> where EnumType : struct
{

    #region Constructor
    public DOMEnumList(Element ownerElement, AtomicName<EAttributeName> localName) : base(ownerElement, localName)
    {
    }

    public DOMEnumList(Element ownerElement, AtomicName<EAttributeName> localName, EnumType[] supportedTokens) : base(ownerElement, localName, supportedTokens)
    {
    }

    protected override EnumType String_To_Token(ReadOnlyMemory<char> str)
    {
        return Lookup.Enum<EnumType>(str);
    }

    protected override string Token_To_String(EnumType token)
    {
        return Lookup.Keyword(token);
    }
    #endregion



}

