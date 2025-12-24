namespace CssUI.CSS.Parser;

/// <summary>
/// Represents the result of matching a token sequence against a CSS grammar production.
/// </summary>
/// <remarks>
/// <para>
/// This struct is used to report whether a token sequence matches productions like
/// <c>&lt;declaration-value&gt;</c> or <c>&lt;any-value&gt;</c>, and if not, provides
/// information about why the match failed.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#any-value"/>
public readonly struct CssProductionMatchResult
{
    #region Properties
    /// <summary>
    /// Gets whether the production matched successfully.
    /// </summary>
    public bool IsMatch { get; }

    /// <summary>
    /// Gets the type of failure if the match was unsuccessful.
    /// </summary>
    /// <remarks>
    /// This is <see cref="ECssProductionMatchFailure.None"/> when <see cref="IsMatch"/> is true.
    /// </remarks>
    public ECssProductionMatchFailure FailureType { get; }

    /// <summary>
    /// Gets the index of the token that caused the failure.
    /// </summary>
    /// <remarks>
    /// This is 0 when <see cref="IsMatch"/> is true.
    /// </remarks>
    public int FailureIndex { get; }

    /// <summary>
    /// Gets a message describing the failure.
    /// </summary>
    /// <remarks>
    /// This is null when <see cref="IsMatch"/> is true.
    /// </remarks>
    public string? FailureMessage { get; }
    #endregion

    #region Constructors
    private CssProductionMatchResult(
        bool isMatch,
        ECssProductionMatchFailure failureType,
        int failureIndex,
        string? failureMessage)
    {
        IsMatch = isMatch;
        FailureType = failureType;
        FailureIndex = failureIndex;
        FailureMessage = failureMessage;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a successful match result.
    /// </summary>
    public static CssProductionMatchResult Success() =>
        new(true, ECssProductionMatchFailure.None, 0, null);

    /// <summary>
    /// Creates a failed match result.
    /// </summary>
    /// <param name="failureType">The type of failure.</param>
    /// <param name="failureIndex">The index of the token that caused the failure.</param>
    /// <param name="failureMessage">A message describing the failure.</param>
    public static CssProductionMatchResult Failure(
        ECssProductionMatchFailure failureType,
        int failureIndex,
        string? failureMessage = null) =>
        new(false, failureType, failureIndex, failureMessage);
    #endregion

    #region Object Overrides
    /// <inheritdoc />
    public override string ToString() =>
        IsMatch
            ? "Match: Success"
            : $"Match: Failed at index {FailureIndex} - {FailureType}: {FailureMessage}";
    #endregion
}
