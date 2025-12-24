namespace CssUI.CSS.Parser;

/// <summary>
/// Specifies the type of content expected in a CSS block, as defined in CSS Syntax Level 3 §8.1.
/// </summary>
/// <remarks>
/// <para>
/// Different CSS grammar productions expect different content inside their blocks:
/// </para>
/// <list type="bullet">
/// <item><description><c>&lt;style-block&gt;</c> - Declarations and nested style rules (style rules, @nest, conditional group rules)</description></item>
/// <item><description><c>&lt;declaration-list&gt;</c> - Only declarations and at-rules (@font-face, @counter-style, @page, @keyframes child rules)</description></item>
/// <item><description><c>&lt;rule-list&gt;</c> - Only qualified rules and at-rules (@keyframes, @font-feature-values)</description></item>
/// <item><description><c>&lt;stylesheet&gt;</c> - Rules with special handling of CDO/CDC tokens (stylesheets, conditional group rules)</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#declaration-rule-list"/>
public enum ECssBlockContentsType
{
    /// <summary>
    /// Contents should be parsed as a <c>&lt;style-block&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepts:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Declarations (property declarations)</description></item>
    /// <item><description>Nested qualified rules (starting with "&amp;")</description></item>
    /// <item><description>At-rules (as defined by the parent rule's grammar)</description></item>
    /// </list>
    /// <para>
    /// Used by: style rules, @nest, nested conditional group rules
    /// </para>
    /// <para>
    /// Parse using: <c>Consume_Style_Block_Contents</c>
    /// </para>
    /// </remarks>
    StyleBlock,

    /// <summary>
    /// Contents should be parsed as a <c>&lt;declaration-list&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepts:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Declarations (descriptor declarations)</description></item>
    /// <item><description>At-rules (as defined by the parent rule's grammar)</description></item>
    /// </list>
    /// <para>
    /// Does NOT accept:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Nested qualified rules (style rules)</description></item>
    /// </list>
    /// <para>
    /// Used by: @font-face, @counter-style, @page, @keyframes child rules
    /// </para>
    /// <para>
    /// Parse using: <c>Consume_Decleration_List</c>
    /// </para>
    /// </remarks>
    DeclarationList,

    /// <summary>
    /// Contents should be parsed as a <c>&lt;rule-list&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepts:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Qualified rules</description></item>
    /// <item><description>At-rules (as defined by the parent rule's grammar)</description></item>
    /// </list>
    /// <para>
    /// Does NOT accept:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Declarations</description></item>
    /// </list>
    /// <para>
    /// Used by: @keyframes, @font-feature-values
    /// </para>
    /// <para>
    /// Parse using: <c>Consume_Rule_List</c> with top-level flag = false
    /// </para>
    /// </remarks>
    RuleList,

    /// <summary>
    /// Contents should be parsed as a <c>&lt;stylesheet&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Accepts:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Qualified rules (interpreted as style rules)</description></item>
    /// <item><description>At-rules (all rules by default, can be restricted)</description></item>
    /// </list>
    /// <para>
    /// Special handling:
    /// </para>
    /// <list type="bullet">
    /// <item><description>CDO and CDC tokens are ignored at top level</description></item>
    /// </list>
    /// <para>
    /// Does NOT accept:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Declarations</description></item>
    /// </list>
    /// <para>
    /// Used by: top-level stylesheets, non-nested conditional group rules (@media, @supports)
    /// </para>
    /// <para>
    /// Parse using: <c>Consume_Rule_List</c> with top-level flag = true
    /// </para>
    /// </remarks>
    Stylesheet
}
