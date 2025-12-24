using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CssUI.CSS.Parser;

/// <summary>
/// Represents a node in a <c>@supports</c> condition tree, as defined in CSS Conditional Rules Level 3 §6.
/// </summary>
/// <remarks>
/// <para>
/// The <c>&lt;supports-condition&gt;</c> is a boolean expression that can contain:
/// </para>
/// <list type="bullet">
/// <item><description>Declaration tests: <c>(property: value)</c></description></item>
/// <item><description>Negations: <c>not (...)</c></description></item>
/// <item><description>Conjunctions: <c>(...) and (...)</c></description></item>
/// <item><description>Disjunctions: <c>(...) or (...)</c></description></item>
/// <item><description>General-enclosed: unknown future syntax (always false)</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public abstract class CssSupportsCondition
{
    /// <summary>
    /// Gets the type of this condition node.
    /// </summary>
    public abstract ESupportsConditionType Type { get; }

    /// <summary>
    /// Evaluates this condition against a CSS feature support checker.
    /// </summary>
    /// <param name="supportsChecker">
    /// A function that determines if a given property:value declaration is supported.
    /// The function receives the property name and value as arguments.
    /// </param>
    /// <returns>True if the condition is satisfied; otherwise, false.</returns>
    public abstract bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker);
}

/// <summary>
/// Represents a declaration test in a <c>@supports</c> condition: <c>( &lt;declaration&gt; )</c>.
/// </summary>
/// <remarks>
/// <para>
/// Tests whether the UA supports a property:value declaration.
/// </para>
/// <para>
/// The declaration value must match the <c>&lt;declaration-value&gt;</c> production,
/// which excludes <c>&lt;bad-string-token&gt;</c>, <c>&lt;bad-url-token&gt;</c>,
/// unmatched <c>&lt;)-token&gt;</c>, <c>&lt;]-token&gt;</c>, or <c>&lt;}-token&gt;</c>,
/// and top-level <c>&lt;semicolon-token&gt;</c> tokens and <c>&lt;delim-token&gt;</c>
/// tokens with a value of "!".
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#typedef-supports-decl"/>
public sealed class CssSupportsDeclaration : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.Declaration;

    /// <summary>
    /// Gets the property name being tested.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Gets the value tokens being tested.
    /// </summary>
    public ImmutableArray<CssToken> Value { get; }

    /// <summary>
    /// Gets whether the declaration includes the <c>!important</c> flag.
    /// </summary>
    public bool IsImportant { get; }

    /// <summary>
    /// Gets whether the declaration value is valid according to the <c>&lt;declaration-value&gt;</c> production.
    /// </summary>
    public bool IsValueValid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsDeclaration"/> class.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="value">The value tokens.</param>
    /// <param name="isImportant">Whether the declaration has !important.</param>
    /// <param name="isValueValid">Whether the value matches &lt;declaration-value&gt; production.</param>
    public CssSupportsDeclaration(string propertyName, ImmutableArray<CssToken> value, bool isImportant, bool isValueValid)
    {
        PropertyName = propertyName;
        Value = value;
        IsImportant = isImportant;
        IsValueValid = isValueValid;
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        // If the value is invalid according to <declaration-value>, the condition is false
        if (!IsValueValid)
            return false;

        return supportsChecker(PropertyName, Value);
    }
}

/// <summary>
/// Represents a negation in a <c>@supports</c> condition: <c>not &lt;supports-in-parens&gt;</c>.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public sealed class CssSupportsNot : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.Not;

    /// <summary>
    /// Gets the child condition being negated.
    /// </summary>
    public CssSupportsCondition Child { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsNot"/> class.
    /// </summary>
    /// <param name="child">The condition to negate.</param>
    public CssSupportsNot(CssSupportsCondition child)
    {
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        return !Child.Evaluate(supportsChecker);
    }
}

/// <summary>
/// Represents a conjunction in a <c>@supports</c> condition: <c>&lt;supports-in-parens&gt; [ and &lt;supports-in-parens&gt; ]*</c>.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public sealed class CssSupportsAnd : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.And;

    /// <summary>
    /// Gets the child conditions that are ANDed together.
    /// </summary>
    public ImmutableArray<CssSupportsCondition> Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsAnd"/> class.
    /// </summary>
    /// <param name="children">The conditions to AND together.</param>
    public CssSupportsAnd(ImmutableArray<CssSupportsCondition> children)
    {
        if (children.Length < 2)
            throw new ArgumentException("AND condition requires at least 2 children", nameof(children));
        Children = children;
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        foreach (var child in Children)
        {
            if (!child.Evaluate(supportsChecker))
                return false;
        }
        return true;
    }
}

/// <summary>
/// Represents a disjunction in a <c>@supports</c> condition: <c>&lt;supports-in-parens&gt; [ or &lt;supports-in-parens&gt; ]*</c>.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public sealed class CssSupportsOr : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.Or;

    /// <summary>
    /// Gets the child conditions that are ORed together.
    /// </summary>
    public ImmutableArray<CssSupportsCondition> Children { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsOr"/> class.
    /// </summary>
    /// <param name="children">The conditions to OR together.</param>
    public CssSupportsOr(ImmutableArray<CssSupportsCondition> children)
    {
        if (children.Length < 2)
            throw new ArgumentException("OR condition requires at least 2 children", nameof(children));
        Children = children;
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        foreach (var child in Children)
        {
            if (child.Evaluate(supportsChecker))
                return true;
        }
        return false;
    }
}

/// <summary>
/// Represents a nested condition in a <c>@supports</c> rule: <c>( &lt;supports-condition&gt; )</c>.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public sealed class CssSupportsNested : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.Nested;

    /// <summary>
    /// Gets the nested condition.
    /// </summary>
    public CssSupportsCondition Child { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsNested"/> class.
    /// </summary>
    /// <param name="child">The nested condition.</param>
    public CssSupportsNested(CssSupportsCondition child)
    {
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        return Child.Evaluate(supportsChecker);
    }
}

/// <summary>
/// Represents an unknown or future <c>&lt;general-enclosed&gt;</c> production in a <c>@supports</c> condition.
/// </summary>
/// <remarks>
/// <para>
/// The <c>&lt;general-enclosed&gt;</c> production is used for forward-compatibility.
/// Any syntax matching this production that is not otherwise recognized evaluates to false.
/// </para>
/// <para>
/// The content must match the <c>&lt;any-value&gt;</c> production.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
/// <seealso href="https://drafts.csswg.org/css-values-4/#typedef-general-enclosed"/>
public sealed class CssSupportsGeneralEnclosed : CssSupportsCondition
{
    /// <inheritdoc/>
    public override ESupportsConditionType Type => ESupportsConditionType.GeneralEnclosed;

    /// <summary>
    /// Gets the raw tokens of the general-enclosed content.
    /// </summary>
    public ImmutableArray<CssToken> Tokens { get; }

    /// <summary>
    /// Gets whether the content is valid according to the <c>&lt;any-value&gt;</c> production.
    /// </summary>
    public bool IsContentValid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CssSupportsGeneralEnclosed"/> class.
    /// </summary>
    /// <param name="tokens">The raw tokens.</param>
    /// <param name="isContentValid">Whether the content matches &lt;any-value&gt; production.</param>
    public CssSupportsGeneralEnclosed(ImmutableArray<CssToken> tokens, bool isContentValid)
    {
        Tokens = tokens;
        IsContentValid = isContentValid;
    }

    /// <inheritdoc/>
    public override bool Evaluate(Func<string, IReadOnlyList<CssToken>, bool> supportsChecker)
    {
        // Per spec: <general-enclosed> always evaluates to false
        // (It exists only for forward-compatibility with future syntax additions)
        return false;
    }
}
