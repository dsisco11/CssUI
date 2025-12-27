using System;

namespace CssUI.DOM;

public class DOMEnumList<EnumType> : AttributeTokenList<EnumType> where EnumType : struct
{

    #region Constructor
    public DOMEnumList(Element ownerElement, string localName) : base(ownerElement, localName)
    {
    }

    public DOMEnumList(Element ownerElement, string localName, EnumType[] supportedTokens) : base(ownerElement, localName, supportedTokens)
    {
    }

    // Convenience overloads for EAttributeName enum
    public DOMEnumList(Element ownerElement, EAttributeName localName) : base(ownerElement, localName)
    {
    }

    public DOMEnumList(Element ownerElement, EAttributeName localName, EnumType[] supportedTokens) : base(ownerElement, localName, supportedTokens)
    {
    }
    #endregion

    protected override EnumType String_To_Token(ReadOnlyMemory<char> str)
    {
        return Lookup.Enum<EnumType>(str.ToString());
    }

    protected override string Token_To_String(EnumType token)
    {
        return Lookup.Keyword(token);
    }

}

