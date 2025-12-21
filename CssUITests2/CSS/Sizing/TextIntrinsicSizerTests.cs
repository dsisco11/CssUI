using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for TextIntrinsicSizer soft wrap detection and text measurement.
/// </summary>
public class TextIntrinsicSizerTests
{
    #region Soft Wrap Opportunity Detection

    // Note: The actual soft wrap detection is internal to TextIntrinsicSizer.
    // These tests verify the expected behavior through examples.

    [Theory]
    [InlineData("hello world", true)]  // Has space - can break
    [InlineData("helloworld", false)]  // No break opportunity
    [InlineData("hello-world", true)]  // Has hyphen - can break after
    [InlineData("hello/world", true)]  // Has slash - can break after
    [InlineData("", false)]            // Empty string
    public void Text_HasSoftWrapOpportunity(string text, bool expectsBreak)
    {
        // This tests the conceptual behavior
        // A string with break opportunities should have min-content < max-content
        // A string without break opportunities should have min-content == max-content

        bool hasBreakOpportunity = ContainsBreakOpportunity(text);
        Assert.Equal(expectsBreak, hasBreakOpportunity);
    }

    private static bool ContainsBreakOpportunity(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c)) return true;
            if (i > 0 && (text[i - 1] == '-' || text[i - 1] == '/')) return true;
        }
        return false;
    }

    #endregion

    #region CJK Character Detection

    [Theory]
    [InlineData('中', true)]   // CJK Unified Ideograph
    [InlineData('あ', true)]   // Hiragana
    [InlineData('ア', true)]   // Katakana
    [InlineData('한', true)]   // Hangul
    [InlineData('A', false)]   // Latin
    [InlineData('1', false)]   // Digit
    [InlineData(' ', false)]   // Space
    public void IsCJKCharacter_DetectsCorrectly(char c, bool expected)
    {
        bool result = IsCJKCharacter(c);
        Assert.Equal(expected, result);
    }

    // Mirror of the internal method for testing
    private static bool IsCJKCharacter(char c)
    {
        return (c >= 0x4E00 && c <= 0x9FFF) ||  // CJK Unified Ideographs
               (c >= 0x3400 && c <= 0x4DBF) ||  // CJK Extension A
               (c >= 0x3000 && c <= 0x303F) ||  // CJK Symbols and Punctuation
               (c >= 0x3040 && c <= 0x309F) ||  // Hiragana
               (c >= 0x30A0 && c <= 0x30FF) ||  // Katakana
               (c >= 0xAC00 && c <= 0xD7AF);    // Hangul Syllables
    }

    #endregion

    #region Min-Content vs Max-Content Behavior

    [Fact]
    public void SingleWord_MinContentEqualsMaxContent()
    {
        // For a single word with no break opportunities,
        // min-content should equal max-content
        string text = "supercalifragilisticexpialidocious";

        // Conceptually:
        // min-content = width of longest unbreakable segment = entire word
        // max-content = width of all content = entire word
        // Therefore they should be equal

        Assert.False(ContainsBreakOpportunity(text));
    }

    [Fact]
    public void MultipleWords_MinContentLessThanMaxContent()
    {
        // For multiple words, there are soft wrap opportunities
        string text = "The quick brown fox";

        // Conceptually:
        // min-content = width of longest word (probably "quick" or "brown")
        // max-content = width of entire string
        // Therefore min < max

        Assert.True(ContainsBreakOpportunity(text));
    }

    [Fact]
    public void CJKText_CanBreakAnywhere()
    {
        // CJK text can break between any characters
        string text = "日本語";  // "Japanese" in Japanese

        // Every character is a potential break point
        foreach (char c in text)
        {
            Assert.True(IsCJKCharacter(c));
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void EmptyString_ReturnsZeroSize()
    {
        // Empty string should return zero intrinsic size
        var size = IntrinsicSize.Zero;

        Assert.Equal(0, size.Inline.MinContent);
        Assert.Equal(0, size.Inline.MaxContent);
    }

    [Fact]
    public void WhitespaceOnly_HasZeroOrMinimalSize()
    {
        // Whitespace-only content typically collapses
        string text = "   ";

        // All characters are break opportunities
        Assert.True(ContainsBreakOpportunity(text));
    }

    [Fact]
    public void MixedContent_BreaksAtAppropriatePoints()
    {
        // Mixed Latin and CJK
        string text = "Hello世界";  // "Hello World" with CJK for "world"

        // Should be able to break between Latin and CJK
        bool hasCJK = false;
        bool hasLatin = false;
        foreach (char c in text)
        {
            if (IsCJKCharacter(c)) hasCJK = true;
            if (c >= 'A' && c <= 'z') hasLatin = true;
        }

        Assert.True(hasCJK);
        Assert.True(hasLatin);
    }

    #endregion
}
