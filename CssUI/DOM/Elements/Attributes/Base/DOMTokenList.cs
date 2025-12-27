using System;

namespace CssUI.DOM;

/// <summary>
/// Represents a set of space-separated tokens (e.g., classList, relList).
/// Spec: https://dom.spec.whatwg.org/#interface-domtokenlist
/// Note: Class tokens are case-sensitive per HTML5 spec.
/// </summary>
public class DOMTokenList : AttributeTokenList<string>
{
    #region Constructor
    public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName) : base(ownerElement, localName)
    {
    }

    public DOMTokenList(Element ownerElement, AtomicName<EAttributeName> localName, string[] supportedTokens) : base(ownerElement, localName, supportedTokens)
    {
    }
    #endregion


    protected override string String_To_Token(ReadOnlyMemory<char> data)
    {
        // Per HTML5 spec, class tokens are case-sensitive
        return data.ToString();
    }

    protected override string Token_To_String(string token)
    {
        return token;
    }

}

