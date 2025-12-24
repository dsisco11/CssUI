using System.Collections.Generic;

namespace CssUI.CSS.Parser;

/// <summary>
/// Interface for collecting CSS parse errors during parsing.
/// </summary>
/// <remarks>
/// <para>
/// Per CSS Syntax Level 3, parse errors are well-defined points in the parsing algorithm.
/// This interface allows consumers to collect and handle these errors as needed.
/// </para>
/// <para>
/// User agents must either act as described in the spec when encountering parse errors,
/// or must abort processing at the first error they encounter. Conformance checkers must
/// report at least one parse error condition if any exist.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#tokenizing-and-parsing"/>
public interface ICssParseErrorReporter
{
    /// <summary>
    /// Reports a parse error.
    /// </summary>
    /// <param name="error">The parse error to report.</param>
    void ReportError(CssParseError error);

    /// <summary>
    /// Gets all reported errors.
    /// </summary>
    IReadOnlyList<CssParseError> Errors { get; }

    /// <summary>
    /// Gets whether any errors have been reported.
    /// </summary>
    bool HasErrors { get; }

    /// <summary>
    /// Clears all reported errors.
    /// </summary>
    void Clear();
}

/// <summary>
/// Default implementation of <see cref="ICssParseErrorReporter"/> that collects all parse errors.
/// </summary>
public sealed class CssParseErrorReporter : ICssParseErrorReporter
{
    private readonly List<CssParseError> _errors = new();

    /// <inheritdoc />
    public IReadOnlyList<CssParseError> Errors => _errors;

    /// <inheritdoc />
    public bool HasErrors => _errors.Count > 0;

    /// <inheritdoc />
    public void ReportError(CssParseError error)
    {
        _errors.Add(error);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _errors.Clear();
    }
}

/// <summary>
/// A null implementation of <see cref="ICssParseErrorReporter"/> that discards all errors.
/// Use this when error reporting is not needed.
/// </summary>
public sealed class NullParseErrorReporter : ICssParseErrorReporter
{
    /// <summary>
    /// Gets the singleton instance of the null reporter.
    /// </summary>
    public static NullParseErrorReporter Instance { get; } = new();

    private static readonly IReadOnlyList<CssParseError> EmptyErrors = new List<CssParseError>();

    private NullParseErrorReporter() { }

    /// <inheritdoc />
    public IReadOnlyList<CssParseError> Errors => EmptyErrors;

    /// <inheritdoc />
    public bool HasErrors => false;

    /// <inheritdoc />
    public void ReportError(CssParseError error) { /* Discard */ }

    /// <inheritdoc />
    public void Clear() { /* Nothing to clear */ }
}
