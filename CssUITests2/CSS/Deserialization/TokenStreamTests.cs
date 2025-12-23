using System;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Deserialization.Tests;

/// <summary>
/// Tests for TokenStream - CSS token stream wrapper around DataConsumer&lt;CssToken&gt;.
/// TokenStream provides iteration, lookahead, consume, and substream functionality for CSS parsing.
/// </summary>
public class TokenStreamTests
{
    #region Test Helpers
    /// <summary>
    /// Creates a TokenStream from CSS text by tokenizing it first.
    /// </summary>
    private static TokenStream CreateStream(string css)
    {
        var tokens = CssTokenizer.Parse(css);
        return new TokenStream(tokens);
    }

    /// <summary>
    /// Creates sample tokens for testing: [Ident("color"), Colon, Ident("red")]
    /// </summary>
    private static CssToken[] CreateSampleTokens()
    {
        return
        [
            new IdentToken("color"),
            ColonToken.Instance,
            new IdentToken("red")
        ];
    }
    #endregion

    #region Constructor Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Constructor_WithTokenArray_CreatesValidStream()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        Assert.NotNull(stream);
        Assert.Equal(tokens.Length, stream.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Constructor_WithReadOnlyMemory_CreatesValidStream()
    {
        var tokens = CreateSampleTokens();
        var memory = new ReadOnlyMemory<CssToken>(tokens);
        var stream = new TokenStream(memory);

        Assert.NotNull(stream);
        Assert.Equal(tokens.Length, stream.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Constructor_WithEmptyArray_CreatesEmptyStream()
    {
        var stream = new TokenStream(Array.Empty<CssToken>());

        Assert.Equal(0, stream.Length);
        Assert.True(stream.atEnd);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Constructor_FromTokenizer_CreatesStreamWithEOF()
    {
        var stream = CreateStream("");

        // Empty CSS should produce single EOF token
        Assert.Equal(1, stream.Length);
        Assert.Equal(ECssTokenType.EOF, stream.Next.Type);
    }
    #endregion

    #region Position Property Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Position_InitiallyZero()
    {
        var stream = CreateStream("color: red");

        Assert.Equal(0, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Position_IncreasesAfterConsume()
    {
        var stream = CreateStream("color: red");
        stream.Consume();

        Assert.Equal(1, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void LongPosition_EqualsPosition()
    {
        var stream = CreateStream("color: red");
        stream.Consume();
        stream.Consume();

        Assert.Equal((ulong)stream.Position, stream.LongPosition);
    }
    #endregion

    #region Length and Remaining Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Length_ReturnsTokenCount()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        Assert.Equal(3, stream.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Remaining_ReturnsUnconsumedCount()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        Assert.Equal(3, stream.Remaining);

        stream.Consume();

        Assert.Equal(2, stream.Remaining);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Remaining_ZeroWhenFullyConsumed()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Consume(3);

        Assert.Equal(0, stream.Remaining);
    }
    #endregion

    #region atEnd and atEOF Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void atEnd_FalseWhenTokensRemaining()
    {
        var stream = CreateStream("color: red");

        Assert.False(stream.atEnd);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void atEnd_TrueWhenNoTokensRemaining()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Consume(3);

        Assert.True(stream.atEnd);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void atEOF_TrueWhenNextIsEOFToken()
    {
        var stream = CreateStream("");

        // Empty CSS produces EOF token
        Assert.True(stream.atEOF);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void atEOF_FalseWhenNextIsNotEOF()
    {
        var stream = CreateStream("color");

        Assert.False(stream.atEOF);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void atEOF_TrueWhenPastEnd()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Consume(3);

        // Past end should return EOF_ITEM which is CssToken.EOF
        Assert.True(stream.atEOF);
    }
    #endregion

    #region Next/NextNext/NextNextNext Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Next_ReturnsFirstToken()
    {
        var stream = CreateStream("color");
        var token = stream.Next;

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("color", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Next_DoesNotAdvancePosition()
    {
        var stream = CreateStream("color: red");
        _ = stream.Next;

        Assert.Equal(0, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void NextNext_ReturnsSecondToken()
    {
        var stream = CreateStream("color:");
        var token = stream.NextNext;

        Assert.Equal(ECssTokenType.Colon, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void NextNextNext_ReturnsThirdToken()
    {
        var stream = CreateStream("color: red");

        // Tokens: Ident("color"), Colon, Whitespace, Ident("red"), EOF
        var token = stream.NextNextNext;

        // Third token should be whitespace
        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Next_AfterConsume_ReturnsNextToken()
    {
        var stream = CreateStream("color:");
        stream.Consume();
        var token = stream.Next;

        Assert.Equal(ECssTokenType.Colon, token.Type);
    }
    #endregion

    #region Peek Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Peek_ZeroOffset_ReturnsNext()
    {
        var stream = CreateStream("color");
        var next = stream.Next;
        var peeked = stream.Peek(0);

        Assert.Same(next, peeked);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Peek_PositiveOffset_ReturnsCorrectToken()
    {
        var stream = CreateStream("color:");
        var token = stream.Peek(1);

        Assert.Equal(ECssTokenType.Colon, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Peek_BeyondEnd_ReturnsEOFToken()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var token = stream.Peek(100);

        Assert.Equal(CssToken.EOF, token);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Peek_NegativeOffset_ThrowsIndexOutOfRangeException()
    {
        var stream = CreateStream("color");

        Assert.Throws<IndexOutOfRangeException>(() => stream.Peek(-1));
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Peek_DoesNotAdvancePosition()
    {
        var stream = CreateStream("color: red");
        _ = stream.Peek(2);

        Assert.Equal(0, stream.Position);
    }
    #endregion

    #region Get Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Get_ZeroIndex_ReturnsFirstToken()
    {
        var stream = CreateStream("color");
        var token = stream.Get(0);

        Assert.Equal(ECssTokenType.Ident, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Get_PositiveIndex_ReturnsTokenAtIndex()
    {
        var stream = CreateStream("color:");
        stream.Consume(); // Move position to 1
        var token = stream.Get(1); // Should still get token at index 1 (not relative)

        Assert.Equal(ECssTokenType.Colon, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Get_BeyondLength_ReturnsEOFToken()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var token = stream.Get(100);

        Assert.Equal(CssToken.EOF, token);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Get_NegativeIndex_ThrowsIndexOutOfRangeException()
    {
        var stream = CreateStream("color");

        Assert.Throws<IndexOutOfRangeException>(() => stream.Get(-1));
    }
    #endregion

    #region Consume Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Consume_ReturnsSingleToken()
    {
        var stream = CreateStream("color");
        var token = stream.Consume();

        Assert.Equal(ECssTokenType.Ident, token.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Consume_AdvancesPosition()
    {
        var stream = CreateStream("color: red");
        stream.Consume();

        Assert.Equal(1, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Consume_Count_ReturnsMultipleTokens()
    {
        var stream = CreateStream("color:");
        var tokens = stream.Consume(2);

        Assert.Equal(2, tokens.Length);
        Assert.Equal(ECssTokenType.Ident, tokens[0].Type);
        Assert.Equal(ECssTokenType.Colon, tokens[1].Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Consume_AtEnd_ReturnsEOFToken()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);
        stream.Consume(3);

        var token = stream.Consume();

        Assert.Equal(CssToken.EOF, token);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Consume_Generic_ReturnsCastToken()
    {
        var stream = CreateStream("color");
        var token = stream.Consume<IdentToken>();

        Assert.Equal("color", token.Value);
    }
    #endregion

    #region Consume_While Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void ConsumeWhile_MatchingPredicate_ConsumesTokens()
    {
        var stream = CreateStream("   color"); // 3 spaces

        // Consume all whitespace tokens
        bool consumed = stream.Consume_While(t => t.Type == ECssTokenType.Whitespace);

        Assert.True(consumed);
        Assert.Equal(ECssTokenType.Ident, stream.Next.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void ConsumeWhile_NoMatch_ReturnsFalse()
    {
        var stream = CreateStream("color");

        bool consumed = stream.Consume_While(t => t.Type == ECssTokenType.Whitespace);

        Assert.False(consumed);
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void ConsumeWhile_WithLimit_StopsAtLimit()
    {
        var stream = CreateStream("      color"); // Multiple spaces

        // Consume at most 2 whitespace tokens
        stream.Consume_While(t => t.Type == ECssTokenType.Whitespace, 2);

        // Should have only consumed up to limit (stream tokenizer combines whitespace so this is just 1 token anyway)
        Assert.Equal(ECssTokenType.Ident, stream.Next.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void ConsumeWhile_OutputsConsumedMemory()
    {
        var tokens = new CssToken[]
        {
            WhitespaceToken.Space,
            WhitespaceToken.Space,
            new IdentToken("test")
        };
        var stream = new TokenStream(tokens);

        stream.Consume_While(t => t.Type == ECssTokenType.Whitespace, out ReadOnlyMemory<CssToken> consumed);

        Assert.Equal(2, consumed.Length);
    }
    #endregion

    #region Seek Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Seek_ToPosition_MovesStreamPosition()
    {
        var stream = CreateStream("color: red");
        stream.Seek(2);

        Assert.Equal(2, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Seek_FromEnd_MovesFromEndOfStream()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Seek(1, FromEnd: true);

        // 3 tokens, position from end 1 = index 2
        Assert.Equal(2, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Seek_ToZero_ResetsPosition()
    {
        var stream = CreateStream("color: red");
        stream.Consume(3);
        stream.Seek(0);

        Assert.Equal(0, stream.Position);
    }
    #endregion

    #region Scan Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Scan_FindsMatchingToken()
    {
        var stream = CreateStream("color:");

        bool found = stream.Scan(ColonToken.Instance, out int offset);

        Assert.True(found);
        Assert.Equal(1, offset); // Colon is at offset 1 from current position
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Scan_NotFound_ReturnsFalse()
    {
        var stream = CreateStream("color");

        bool found = stream.Scan(SemicolonToken.Instance, out int offset);

        Assert.False(found);
        Assert.Equal(0, offset);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Scan_WithPredicate_FindsMatch()
    {
        var stream = CreateStream("color: red");

        bool found = stream.Scan(t => t.Type == ECssTokenType.Colon, out int offset);

        Assert.True(found);
        Assert.Equal(1, offset);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Scan_WithStartOffset_SearchesFromOffset()
    {
        var stream = CreateStream("color: red:");

        // Skip the first colon, find the second
        bool found = stream.Scan(t => t.Type == ECssTokenType.Colon, out int offset, 2);

        Assert.True(found);
        Assert.True(offset > 2); // Should be past the start offset
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Scan_DoesNotAdvancePosition()
    {
        var stream = CreateStream("color:");
        stream.Scan(ColonToken.Instance, out int _);

        Assert.Equal(0, stream.Position);
    }
    #endregion

    #region Reconsume Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Reconsume_MovesPositionBack()
    {
        var stream = CreateStream("color:");
        stream.Consume();
        stream.Reconsume();

        Assert.Equal(0, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Reconsume_MultipleItems_MovesBackByCount()
    {
        var stream = CreateStream("color: red");
        stream.Consume(3);
        stream.Reconsume(2);

        Assert.Equal(1, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Reconsume_ExceedsConsumed_ThrowsException()
    {
        var stream = CreateStream("color");
        stream.Consume();

        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Reconsume(2));
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Reconsume_AllowsRereadingTokens()
    {
        var stream = CreateStream("color");
        var first = stream.Consume();
        stream.Reconsume();
        var second = stream.Next;

        Assert.Same(first, second);
    }
    #endregion

    #region Substream Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_Count_ReturnsNewStreamWithTokens()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var substream = stream.Substream(2);

        Assert.Equal(2, substream.Length);
        Assert.Equal(ECssTokenType.Ident, substream.Next.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_AdvancesOriginalStreamPosition()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Substream(2);

        Assert.Equal(2, stream.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_ExceedsRemaining_ThrowsException()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Substream(100));
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_WithOffset_SkipsTokens()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var substream = stream.Substream(1, 1); // Skip 1, take 1

        Assert.Equal(1, substream.Length);
        Assert.Equal(ECssTokenType.Colon, substream.Next.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_WithPredicate_ConsumesMatchingTokens()
    {
        var tokens = new CssToken[]
        {
            new IdentToken("a"),
            new IdentToken("b"),
            ColonToken.Instance
        };
        var stream = new TokenStream(tokens);

        var substream = stream.Substream(t => t.Type == ECssTokenType.Ident);

        Assert.Equal(2, substream.Length);
        Assert.Equal(ECssTokenType.Colon, stream.Next.Type); // Original stream advanced
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Substream_InheritsEOFItem()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var substream = stream.Substream(2);

        // When substream is exhausted, it should return EOF
        substream.Consume(2);
        Assert.True(substream.atEOF);
    }
    #endregion

    #region Slice Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Slice_ReturnsRemainingTokens()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);
        stream.Consume();

        var slice = stream.Slice();

        Assert.Equal(2, slice.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Slice_WithOffset_SkipsTokens()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var slice = stream.Slice(1);

        Assert.Equal(2, slice.Length); // Skip 1, get remaining 2
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Slice_WithOffsetAndCount_ReturnsSpecificRange()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var slice = stream.Slice(1, 1);

        Assert.Equal(1, slice.Length);
        Assert.Equal(ECssTokenType.Colon, slice.Span[0].Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Slice_DoesNotAdvancePosition()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        stream.Slice(2);

        Assert.Equal(0, stream.Position);
    }
    #endregion

    #region Clone Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void Clone_CreatesCopyWithSamePosition()
    {
        var stream = CreateStream("color: red");
        stream.Consume(2);

        var clone = stream.Clone();

        Assert.Equal(stream.Position, clone.Position);
        Assert.Equal(stream.Length, clone.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Clone_IndependentFromOriginal()
    {
        var stream = CreateStream("color: red");
        var clone = stream.Clone();

        stream.Consume();

        Assert.NotEqual(stream.Position, clone.Position);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void Clone_SharesUnderlyingData()
    {
        var stream = CreateStream("color");
        var clone = stream.Clone();

        // Both should reference the same token instances
        Assert.Same(stream.Next, clone.Next);
    }
    #endregion

    #region AsMemory and AsSpan Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    public void AsMemory_ReturnsFullTokenData()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var memory = stream.AsMemory();

        Assert.Equal(3, memory.Length);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    public void AsSpan_ReturnsFullTokenData()
    {
        var tokens = CreateSampleTokens();
        var stream = new TokenStream(tokens);

        var span = stream.AsSpan();

        Assert.Equal(3, span.Length);
    }
    #endregion

    #region Integration Tests
    [Fact]
    [Trait("Category", "TokenStream")]
    [Trait("Category", "Integration")]
    public void TokenStream_ParseDeclaration_ConsumeWhilePattern()
    {
        var stream = CreateStream("color: red;");

        // Skip leading whitespace
        stream.Consume_While(t => t.Type == ECssTokenType.Whitespace);

        // Consume property name
        var propertyToken = stream.Consume<IdentToken>();
        Assert.Equal("color", propertyToken.Value);

        // Consume colon
        var colonToken = stream.Consume();
        Assert.Equal(ECssTokenType.Colon, colonToken.Type);

        // Skip whitespace after colon
        stream.Consume_While(t => t.Type == ECssTokenType.Whitespace);

        // Consume value
        var valueToken = stream.Consume<IdentToken>();
        Assert.Equal("red", valueToken.Value);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    [Trait("Category", "Integration")]
    public void TokenStream_ScanForSemicolon_FindsEndOfDeclaration()
    {
        var stream = CreateStream("color: red; background: blue");

        bool found = stream.Scan(t => t.Type == ECssTokenType.Semicolon, out int semicolonOffset);

        Assert.True(found);

        // Consume up to (but not including) the semicolon
        stream.Consume(semicolonOffset);

        Assert.Equal(ECssTokenType.Semicolon, stream.Next.Type);
    }

    [Fact]
    [Trait("Category", "TokenStream")]
    [Trait("Category", "Integration")]
    public void TokenStream_NestedSubstreams_WorkCorrectly()
    {
        var stream = CreateStream("a { b: c; }");

        // Skip to opening brace
        while (stream.Next.Type != ECssTokenType.Bracket_Open && !stream.atEnd)
        {
            stream.Consume();
        }

        // Consume opening brace
        stream.Consume();

        // Find closing brace position
        stream.Scan(t => t.Type == ECssTokenType.Bracket_Close, out int closeOffset);

        // Create substream for block content
        var blockStream = stream.Substream(closeOffset);

        Assert.True(blockStream.Length > 0);
        Assert.Equal(ECssTokenType.Bracket_Close, stream.Next.Type); // Original stream at close brace
    }
    #endregion
}
