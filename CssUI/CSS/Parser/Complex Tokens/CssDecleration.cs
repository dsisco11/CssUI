using System;
using System.Collections.Generic;
using System.Text;
using CssUI.CSS.Parser;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS declaration (property: value).
/// </summary>
/// <remarks>
/// <para>
/// A declaration consists of a property name, a value (sequence of component values),
/// and an optional !important flag.
/// </para>
/// <para>
/// For custom properties (names starting with "--"), the value is validated against
/// the <c>&lt;declaration-value&gt;</c> production per CSS Syntax Level 3 §8.2.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#declaration"/>
public class CssDecleration : CssComponent
{
    #region Properties
    /// <summary>
    /// Gets the property name of this declaration.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Gets the list of component values that make up this declaration's value.
    /// </summary>
    public List<CssToken> Values = new List<CssToken>();

    /// <summary>
    /// Gets or sets whether this declaration has the !important flag.
    /// </summary>
    public bool Important = false;

    /// <summary>
    /// Gets whether this declaration is for a custom property (name starts with "--").
    /// </summary>
    /// <remarks>
    /// Custom properties are case-sensitive and their values must match
    /// the <c>&lt;declaration-value&gt;</c> production.
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-variables-1/#defining-variables"/>
    public bool IsCustomProperty => Name.StartsWith("--", StringComparison.Ordinal);

    /// <summary>
    /// Gets the validation result for the declaration's value against the
    /// <c>&lt;declaration-value&gt;</c> production.
    /// </summary>
    /// <remarks>
    /// This is primarily used for custom properties where the value must be validated.
    /// For standard properties, this will typically be a success result.
    /// </remarks>
    public CssProductionMatchResult ValueValidation { get; internal set; } = CssProductionMatchResult.Success();

    /// <summary>
    /// Gets whether the declaration's value is valid according to the
    /// <c>&lt;declaration-value&gt;</c> production.
    /// </summary>
    /// <remarks>
    /// A declaration with an invalid value should be treated as having no value
    /// per CSS Syntax Level 3.
    /// </remarks>
    public bool IsValueValid => ValueValidation.IsMatch;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new CSS declaration with the specified property name.
    /// </summary>
    /// <param name="Name">The property name.</param>
    public CssDecleration(ReadOnlySpan<char> Name) : base(ECssTokenType.Decleration)
    {
        this.Name = Name.ToString();
    }
    #endregion

    #region Methods
    /// <inheritdoc />
    public override string Encode()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Name);
        sb.Append(": ");
        foreach (CssToken t in Values) { sb.Append(t.Encode()); }
        if (Important) sb.Append(" !important");
        sb.Append(";");

        return sb.ToString();
    }
    #endregion
}

