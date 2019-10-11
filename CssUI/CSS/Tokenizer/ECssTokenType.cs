namespace CssUI.CSS.Parser
{
    /// <summary>
    /// Defines all of the different possible CSS token types
    /// </summary>
    public enum ECssTokenType
    {
        /// <summary></summary>
        None,
        /// <summary>Represents a collection of 2 or more characters NOT within quotations</summary>
        Ident,
        /// <summary>Represents an identifier followed immediately by an opening-parenthesis "FUNCTION_NAME("</summary>
        FunctionName,
        /// <summary>Represents an identifier prefixed with an at-sign(@)</summary>
        At_Keyword,
        /// <summary>Represents an identifier prefixed with a hashtag(#)</summary>
        Hash,
        /// <summary>Represents a collection of characters contained within quotations ("")</summary>
        String,
        /// <summary></summary>
        Bad_String,
        /// <summary></summary>
        Url,
        /// <summary></summary>
        Bad_Url,
        /// <summary>Represents any single character not assigned to another token</summary>
        Delim,
        /// <summary></summary>
        Number,
        /// <summary></summary>
        Percentage,
        /// <summary></summary>
        Dimension,
        /// <summary></summary>
        Unicode_Range,
        /// <summary>Represents '~='</summary>
        Include_Match,
        /// <summary>Represents '|='</summary>
        Dash_Match,
        /// <summary>Represents '^='</summary>
        Prefix_Match,
        /// <summary>Represents '$='</summary>
        Suffix_Match,
        /// <summary>Represents '*='</summary>
        Substring_Match,
        /// <summary>Represents '|'</summary>
        Column,
        /// <summary>Represents any of ' ', '\n', '\r', '\t', '\f'</summary>
        Whitespace,
        /// <summary>Represents '<!--'</summary>
        CDO,
        /// <summary>Represents '-->'</summary>
        CDC,
        /// <summary>Represents ':'</summary>
        Colon,
        /// <summary>Represents ';'</summary>
        Semicolon,
        /// <summary>Represents ','</summary>
        Comma,
        /// <summary>Represents '['</summary>
        SqBracket_Open,
        /// <summary>Represents ']'</summary>
        SqBracket_Close,
        /// <summary>Represents '('</summary>
        Parenth_Open,
        /// <summary>Represents ')'</summary>
        Parenth_Close,
        /// <summary>Represents '{'</summary>
        Bracket_Open,
        /// <summary>Represents '}'</summary>
        Bracket_Close,
        /// <summary>Represents an EOF token</summary>
        EOF,

        // EXTENDED TOKENS
        Function,
        Decleration,
        SimpleBlock,
        AtRule,
        QualifiedRule,

        /// <summary>Represents a namespace prefix for a selector</summary>
        NamespacePrefix,
        /// <summary>Represents a qualified name for a selector</summary>
        QualifiedName,
        /// <summary>Represents a selector combinator</summary>
        Combinator,
    }
}
