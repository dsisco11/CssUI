using System;
using System.Linq;
using CssUI.CSS.Exceptions;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for CSS parse error recovery infrastructure per CSS Syntax Level 3.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#error-handling"/>
public class CssParseErrorRecoveryTests
{
    #region Error Reporter Infrastructure Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseErrorReporter_Collects_Errors()
    {
        // Arrange
        var reporter = new CssParseErrorReporter();

        // Act
        reporter.ReportError(CssParseError.BadString(0));
        reporter.ReportError(CssParseError.BadUrl(5));

        // Assert
        Assert.True(reporter.HasErrors);
        Assert.Equal(2, reporter.Errors.Count);
        Assert.Equal(ECssParseErrorType.BadString, reporter.Errors[0].ErrorType);
        Assert.Equal(ECssParseErrorType.BadUrl, reporter.Errors[1].ErrorType);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseErrorReporter_Clear_Removes_All_Errors()
    {
        // Arrange
        var reporter = new CssParseErrorReporter();
        reporter.ReportError(CssParseError.BadString(0));
        reporter.ReportError(CssParseError.BadUrl(5));

        // Act
        reporter.Clear();

        // Assert
        Assert.False(reporter.HasErrors);
        Assert.Empty(reporter.Errors);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void NullParseErrorReporter_Discards_Errors()
    {
        // Arrange & Act
        var reporter = NullParseErrorReporter.Instance;
        reporter.ReportError(CssParseError.BadString(0));
        reporter.ReportError(CssParseError.BadUrl(5));

        // Assert
        Assert.False(reporter.HasErrors);
        Assert.Empty(reporter.Errors);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void NullParseErrorReporter_Is_Singleton()
    {
        // Arrange & Act
        var instance1 = NullParseErrorReporter.Instance;
        var instance2 = NullParseErrorReporter.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    #endregion

    #region CssParseError Factory Methods Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_UnterminatedString_HasCorrectProperties()
    {
        // Act
        var error = CssParseError.UnterminatedString(10, "'test");

        // Assert
        Assert.Equal(ECssParseErrorType.UnterminatedString, error.ErrorType);
        Assert.Equal(10, error.TokenIndex);
        Assert.Equal("'test", error.Context);
        Assert.Contains("terminated", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_UnterminatedAtRule_HasCorrectProperties()
    {
        // Act
        var error = CssParseError.UnterminatedAtRule(15, "media");

        // Assert
        Assert.Equal(ECssParseErrorType.UnterminatedAtRule, error.ErrorType);
        Assert.Equal(15, error.TokenIndex);
        Assert.Equal("media", error.Context);
        Assert.Contains("@media", error.Message);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_UnterminatedBlock_HasCorrectProperties()
    {
        // Act
        var error = CssParseError.UnterminatedBlock(20, '{');

        // Assert
        Assert.Equal(ECssParseErrorType.UnterminatedBlock, error.ErrorType);
        Assert.Equal(20, error.TokenIndex);
        Assert.Equal("{", error.Context);
        Assert.Contains("{", error.Message);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_MissingColonInDeclaration_HasCorrectProperties()
    {
        // Act
        var error = CssParseError.MissingColonInDeclaration(25, "color");

        // Assert
        Assert.Equal(ECssParseErrorType.MissingColonInDeclaration, error.ErrorType);
        Assert.Equal(25, error.TokenIndex);
        Assert.Equal("color", error.Context);
        Assert.Contains("colon", error.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_UnmatchedClosingBracket_HasCorrectProperties()
    {
        // Act
        var error = CssParseError.UnmatchedClosingBracket(30, '}');

        // Assert
        Assert.Equal(ECssParseErrorType.UnmatchedClosingBracket, error.ErrorType);
        Assert.Equal(30, error.TokenIndex);
        Assert.Equal("}", error.Context);
        Assert.Contains("}", error.Message);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParseError_ToString_FormatsCorrectly()
    {
        // Arrange
        var error = new CssParseError(
            ECssParseErrorType.BadString,
            "Test message",
            line: 5,
            column: 10,
            tokenIndex: 42
        );

        // Act
        var result = error.ToString();

        // Assert
        Assert.Contains("[BadString]", result);
        Assert.Contains("line 5", result);
        Assert.Contains("column 10", result);
        Assert.Contains("Test message", result);
    }

    #endregion

    #region Parser Error Mode Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_DefaultErrorMode_IsRecover()
    {
        // Arrange & Act
        var parser = new CssParser("body { color: red }".AsSpan());

        // Assert
        Assert.Equal(EParseErrorMode.Recover, parser.ErrorMode);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_WithErrorReporter_UsesProvidedReporter()
    {
        // Arrange
        var reporter = new CssParseErrorReporter();

        // Act
        var parser = new CssParser("body { color: red }".AsSpan(), reporter);

        // Assert
        Assert.Same(reporter, parser.ErrorReporter);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_WithoutErrorReporter_UsesNullReporter()
    {
        // Arrange & Act
        var parser = new CssParser("body { color: red }".AsSpan());

        // Assert - should use NullParseErrorReporter
        Assert.NotNull(parser.ErrorReporter);
        Assert.False(parser.ErrorReporter.HasErrors);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_AbortMode_CanBeSet()
    {
        // Arrange & Act
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(
            "body { color: red }".AsSpan(),
            reporter,
            EParseErrorMode.Abort
        );

        // Assert
        Assert.Equal(EParseErrorMode.Abort, parser.ErrorMode);
    }

    #endregion

    #region Error Recovery Behavior Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_RecoverMode_ContinuesParsing_AfterInvalidDeclaration()
    {
        // Arrange - invalid declaration followed by valid one
        var css = "body { color red; background: blue; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var rules = parser.Parse_Rule_List().ToList();

        // Assert - should have parsed something despite the error
        Assert.NotEmpty(rules);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_ParseStylesheet_HandlesEmptyInput()
    {
        // Arrange
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser("".AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Rules);
        Assert.False(reporter.HasErrors);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_ParseStylesheet_HandlesWhitespaceOnly()
    {
        // Arrange
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser("   \t\n   ".AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Rules);
        Assert.False(reporter.HasErrors);
    }

    #endregion

    #region Unmatched Closing Bracket Error Tests (§5 Parsing)

    /// <summary>
    /// Per CSS Syntax Level 3: "The tokens &lt;}-token&gt;, &lt;)-token&gt;, &lt;]-token&gt;,
    /// &lt;bad-string-token&gt;, and &lt;bad-url-token&gt; are always parse errors, but they
    /// are preserved in the token stream."
    /// </summary>
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingBrace_ReportsError_InStylesheet()
    {
        // Arrange - unmatched } at top level
        var css = "body { color: red; } } .class { color: blue; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should report unmatched closing bracket error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket);
        Assert.Contains(reporter.Errors, e => e.Context == "}");
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingParen_ReportsError_InStylesheet()
    {
        // Arrange - unmatched ) at top level
        var css = "body { color: red; } ) .class { color: blue; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should report unmatched closing bracket error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket);
        Assert.Contains(reporter.Errors, e => e.Context == ")");
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingSquareBracket_ReportsError_InStylesheet()
    {
        // Arrange - unmatched ] at top level
        var css = "body { color: red; } ] .class { color: blue; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should report unmatched closing bracket error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket);
        Assert.Contains(reporter.Errors, e => e.Context == "]");
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingBracket_RecoversContinuesParsing()
    {
        // Arrange - unmatched } followed by valid rule
        var css = "} .valid { color: green; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should recover and parse the valid rule
        Assert.True(reporter.HasErrors);
        Assert.NotEmpty(result.Rules);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_MultipleUnmatchedClosingBrackets_ReportsMultipleErrors()
    {
        // Arrange - multiple unmatched closing brackets
        var css = "} ) ] .valid { color: green; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should report all three errors
        Assert.True(reporter.HasErrors);
        var unmatchedErrors = reporter.Errors.Where(e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket).ToList();
        Assert.Equal(3, unmatchedErrors.Count);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingBrace_InDeclarationList_ReportsError()
    {
        // Arrange - unmatched } in declaration list context
        var css = "color: red; } background: blue;";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Decleration_List().ToList();

        // Assert - should report unmatched closing bracket error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnmatchedClosingBrace_InStyleBlockContents_ReportsError()
    {
        // Arrange - unmatched } in style block contents
        var css = "color: red; } background: blue;";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Style_Block_Contents().ToList();

        // Assert - should report unmatched closing bracket error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnmatchedClosingBracket);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_MatchedBrackets_NoError()
    {
        // Arrange - properly matched brackets
        var css = "body { color: rgb(255, 0, 0); content: '[test]'; }";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - no errors for properly matched brackets
        Assert.False(reporter.HasErrors);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void CssParser_UnterminatedBlock_ReportsError()
    {
        // Arrange - unterminated block (missing closing brace)
        var css = "body { color: red;";
        var reporter = new CssParseErrorReporter();
        var parser = new CssParser(css.AsSpan(), reporter);

        // Act
        var result = parser.Parse_Stylesheet();

        // Assert - should report unterminated block error
        Assert.True(reporter.HasErrors);
        Assert.Contains(reporter.Errors, e => e.ErrorType == ECssParseErrorType.UnterminatedBlock);
    }

    #endregion

    #region ECssParseErrorType Enum Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "ErrorRecovery")]
    public void ECssParseErrorType_HasExpectedValues()
    {
        // Assert all expected error types exist
        Assert.Equal(0, (int)ECssParseErrorType.None);
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.BadString));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.BadUrl));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedComment));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedString));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedUrl));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedAtRule));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedQualifiedRule));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedBlock));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnterminatedFunction));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.MissingColonInDeclaration));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.UnmatchedClosingBracket));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.BadStringToken));
        Assert.True(Enum.IsDefined(typeof(ECssParseErrorType), ECssParseErrorType.BadUrlToken));
    }

    #endregion
}
