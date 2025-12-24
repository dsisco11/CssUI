namespace CssUI.CSS.Parser;

/// <summary>
/// Specifies the type of a <c>&lt;supports-condition&gt;</c> node, as defined in CSS Conditional Rules Level 3 §6.
/// </summary>
/// <remarks>
/// <para>
/// The <c>@supports</c> rule condition is a boolean expression composed of these node types:
/// </para>
/// <list type="bullet">
/// <item><description><c>not &lt;supports-in-parens&gt;</c> - Negation</description></item>
/// <item><description><c>&lt;supports-in-parens&gt; [ and &lt;supports-in-parens&gt; ]*</c> - Conjunction</description></item>
/// <item><description><c>&lt;supports-in-parens&gt; [ or &lt;supports-in-parens&gt; ]*</c> - Disjunction</description></item>
/// <item><description><c>( &lt;declaration&gt; )</c> - Feature query (declaration test)</description></item>
/// <item><description><c>&lt;general-enclosed&gt;</c> - Unknown/future syntax (always false)</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public enum ESupportsConditionType
{
    /// <summary>
    /// A declaration test: <c>( &lt;declaration&gt; )</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tests whether the UA supports a given property:value declaration.
    /// The result is true if the declaration is supported.
    /// </para>
    /// <para>
    /// Example: <c>(display: flex)</c>
    /// </para>
    /// </remarks>
    Declaration,

    /// <summary>
    /// A negation: <c>not &lt;supports-in-parens&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result is the logical negation of the child condition.
    /// </para>
    /// <para>
    /// Example: <c>not (display: grid)</c>
    /// </para>
    /// </remarks>
    Not,

    /// <summary>
    /// A conjunction: <c>&lt;supports-in-parens&gt; [ and &lt;supports-in-parens&gt; ]*</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result is true if all child conditions are true.
    /// </para>
    /// <para>
    /// Example: <c>(display: flex) and (gap: 1rem)</c>
    /// </para>
    /// </remarks>
    And,

    /// <summary>
    /// A disjunction: <c>&lt;supports-in-parens&gt; [ or &lt;supports-in-parens&gt; ]*</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The result is true if any child condition is true.
    /// </para>
    /// <para>
    /// Example: <c>(display: flex) or (display: grid)</c>
    /// </para>
    /// </remarks>
    Or,

    /// <summary>
    /// A nested condition: <c>( &lt;supports-condition&gt; )</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contains a nested condition wrapped in parentheses.
    /// The result is the result of the nested condition.
    /// </para>
    /// <para>
    /// Example: <c>((display: flex))</c>
    /// </para>
    /// </remarks>
    Nested,

    /// <summary>
    /// A general-enclosed production: <c>&lt;general-enclosed&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Represents an unknown syntax that matches the <c>&lt;general-enclosed&gt;</c> grammar.
    /// This is used for forward compatibility with future @supports extensions.
    /// The result is always false.
    /// </para>
    /// <para>
    /// Example: <c>selector(:has(> .foo))</c> (before :has() was supported)
    /// </para>
    /// </remarks>
    /// <seealso href="https://drafts.csswg.org/css-values-4/#typedef-general-enclosed"/>
    GeneralEnclosed
}
