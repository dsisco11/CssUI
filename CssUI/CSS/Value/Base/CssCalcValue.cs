using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for calc() expressions, avoiding boxing of the expression reference.
/// </summary>
/// <remarks>
/// <para>
/// CSS calc() expressions are used for dynamic calculations in properties like
/// <c>width</c>, <c>height</c>, <c>margin</c>, <c>padding</c>, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#calc-func
/// </para>
/// <para>
/// This subclass stores the <see cref="CssCalcExpression"/> directly, providing
/// type-safe access without boxing.
/// </para>
/// </remarks>
public sealed record class CssCalcValue : CssValue
{
    private readonly CssCalcExpression _expression;

    /// <summary>
    /// Gets the calc() expression.
    /// </summary>
    public CssCalcExpression Expression => _expression;

    /// <inheritdoc/>
    public override bool HasValue => _expression is not null;

    /// <summary>
    /// Creates a new <see cref="CssCalcValue"/> with the specified calc expression.
    /// </summary>
    /// <param name="expression">The calc expression.</param>
    internal CssCalcValue(CssCalcExpression expression) : base(ECssValueTypes.CALC)
    {
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: calc(...)
    /// </remarks>
    public override string ToString() => _expression.ToCssString();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _expression.ToCssString();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS calc() function per CSS Values Level 4.
    /// </remarks>
    public override string Serialize() => _expression.ToCssString();

    /// <summary>
    /// Returns the calc expression directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssCalcExpression AsCalcExpression() => _expression;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        // Fall back to string allocation for complex expression serialization
        var serialized = _expression.ToCssString();
        if (destination.Length < serialized.Length)
        {
            charsWritten = 0;
            return false;
        }

        serialized.AsSpan().CopyTo(destination);
        charsWritten = serialized.Length;
        return true;
    }
}
