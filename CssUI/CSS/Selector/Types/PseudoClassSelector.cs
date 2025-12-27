using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;


public class PseudoClassSelector : SimpleSelector
{
    /// <summary>
    /// The pseudo class name
    /// </summary>
    protected readonly string Name;

    public PseudoClassSelector(string PseudoClass) : base(ESimpleSelectorType.PseudoClassSelector)
    {
        Name = PseudoClass;
    }

    public static PseudoClassSelector Create_Function(string Name, CssToken[]? Args = null)
    {
        if (Name.Equals("not"))
        {
            return new PseudoClassSelectorNegationFunction(Name, new DataConsumer<CssToken>(Args ?? Array.Empty<CssToken>(), CssToken.EOF));
        }
        else if (Name.StartsWith("nth-"))
        {
            return new PseudoClassSelectorAnBFunction(Name, new DataConsumer<CssToken>(Args ?? Array.Empty<CssToken>(), CssToken.EOF));
        }

        return new PseudoClassSelectorFunction(Name, new List<CssToken>(Args ?? Array.Empty<CssToken>()));
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        switch (Name)
        {
            case "hover":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "active":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "focus":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "enabled":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "disabled":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "drop":
                throw new NotImplementedException($"Pseudo-class selector logic for ':{Name}' has not yet been implemented");
            case "checked":
                {
                    return (E.hasAttribute(EAttributeName.Checked, out Attr outChecked) && !string.IsNullOrEmpty(outChecked.Value.AsString()) == true);
                }
            case "indeterminate":
                {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#indeterminate
                    return (E.hasAttribute(EAttributeName.Checked, out Attr outChecked) && outChecked.Value.AsString().Equals("2"));
                }
            case "empty":
                return !E.hasChildNodes();
            case "root":
                {
                    // Per CSS Selectors Level 4: :root matches the root element of the document
                    // For HTML documents, this is the <html> element (the documentElement)
                    var ownerDocument = E.ownerDocument;
                    if (ownerDocument == null)
                    {
                        // If no owner document, check if this element has no parent (is its own root)
                        return E.parentNode == null || E.parentNode is Document;
                    }
                    return ReferenceEquals(E, ownerDocument.documentElement);
                }
            case "first-child":
                {
                    // Per CSS Selectors Level 4: :first-child matches element that is the first child of its parent
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    return ReferenceEquals(E, parent.firstElementChild);
                }
            case "last-child":
                {
                    // Per CSS Selectors Level 4: :last-child matches element that is the last child of its parent
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    return ReferenceEquals(E, parent.lastElementChild);
                }
            case "only-child":
                {
                    // Per CSS Selectors Level 4: :only-child matches element that is the only child of its parent
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    return ReferenceEquals(E, parent.firstElementChild) && ReferenceEquals(E, parent.lastElementChild);
                }
            case "first-of-type":
                {
                    // Per CSS Selectors Level 4: :first-of-type matches element that is first sibling of its type
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    foreach (var sibling in parent.children)
                    {
                        if (sibling.localName == E.localName)
                        {
                            return ReferenceEquals(E, sibling);
                        }
                    }
                    return false;
                }
            case "last-of-type":
                {
                    // Per CSS Selectors Level 4: :last-of-type matches element that is last sibling of its type
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    Element? lastOfType = null;
                    foreach (var sibling in parent.children)
                    {
                        if (sibling.localName == E.localName)
                        {
                            lastOfType = sibling;
                        }
                    }
                    return ReferenceEquals(E, lastOfType);
                }
            case "only-of-type":
                {
                    // Per CSS Selectors Level 4: :only-of-type matches element that is only sibling of its type
                    var parent = E.parentElement;
                    if (parent == null) return false;
                    int count = 0;
                    foreach (var sibling in parent.children)
                    {
                        if (sibling.localName == E.localName)
                        {
                            count++;
                            if (count > 1) return false;
                        }
                    }
                    return count == 1;
                }
            default:
                throw new CssSelectorException("Selector pseudo-class (", Name, ") logic not implemented!");
        }
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: pseudo-class serializes as ":" followed by the name
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = ':';
        charsWritten = 1;

        if (!Name.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += Name.Length;

        return true;
    }
    #endregion
}

