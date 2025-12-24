using System;

namespace CssUI.CSS;

/// <summary>
/// Represents an An+B value from the CSS An+B microsyntax.
/// </summary>
/// <remarks>
/// <para>
/// The An+B notation defines an integer step (A) and offset (B), and represents the An+Bth
/// elements in a list, for every positive integer or zero value of n, with the first element
/// in the list having index 1 (not 0).
/// </para>
/// <para>
/// For values of A and B greater than 0, this effectively divides the list into groups of
/// A elements (the last group taking the remainder), and selects the Bth element of each group.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#anb-microsyntax"/>
public readonly struct CssAnB : ISpanFormattable
{
    /// <summary>
    /// The step value (coefficient of n).
    /// </summary>
    public readonly int A;

    /// <summary>
    /// The offset value.
    /// </summary>
    public readonly int B;

    /// <summary>
    /// Creates a new An+B value.
    /// </summary>
    /// <param name="a">The step value (coefficient of n).</param>
    /// <param name="b">The offset value.</param>
    public CssAnB(int a, int b)
    {
        A = a;
        B = b;
    }

    /// <summary>
    /// The An+B value representing 'odd' (2n+1).
    /// </summary>
    public static readonly CssAnB Odd = new(2, 1);

    /// <summary>
    /// The An+B value representing 'even' (2n).
    /// </summary>
    public static readonly CssAnB Even = new(2, 0);

    /// <summary>
    /// Checks if a given 1-based index matches this An+B pattern.
    /// </summary>
    /// <param name="index">The 1-based index to check.</param>
    /// <returns>True if the index matches the An+B pattern; otherwise, false.</returns>
    /// <remarks>
    /// The first element in a list has index 1 (not 0).
    /// A match occurs when there exists a non-negative integer n such that An+B equals the index.
    /// </remarks>
    public bool Matches(int index)
    {
        // If both A and B are 0, no element matches
        if (A == 0 && B == 0)
        {
            return false;
        }

        // If A is 0, only the B-th element matches
        if (A == 0)
        {
            return index == B;
        }

        // Check if (index - B) is divisible by A and the resulting n is non-negative
        int diff = index - B;

        // If A and diff have different signs (and diff != 0), no non-negative n exists
        if (A > 0)
        {
            // n = (index - B) / A must be >= 0
            // This means index >= B when A > 0
            if (diff < 0)
            {
                return false;
            }
            return diff % A == 0;
        }
        else // A < 0
        {
            // n = (index - B) / A, and n >= 0
            // Since A < 0, diff must be <= 0 for n >= 0
            if (diff > 0)
            {
                return false;
            }
            return diff % A == 0;
        }
    }

    /// <summary>
    /// Serializes this An+B value to its canonical CSS string representation.
    /// </summary>
    /// <returns>The canonical serialization of this An+B value.</returns>
    /// <remarks>
    /// Serialization follows CSS Syntax Level 3 §10.1.
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#serializing-anb"/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        // 1. If A is zero, return the serialization of B.
        if (A == 0)
        {
            return B.TryFormat(destination, out charsWritten, default, provider);
        }

        // 2. Handle A value
        if (A == 1)
        {
            // Append "n" to result.
            if (destination.Length < 1) return false;
            destination[0] = 'n';
            charsWritten = 1;
        }
        else if (A == -1)
        {
            // Append "-n" to result.
            if (destination.Length < 2) return false;
            destination[0] = '-';
            destination[1] = 'n';
            charsWritten = 2;
        }
        else
        {
            // Serialize A and append it to result, then append "n" to result.
            if (!A.TryFormat(destination, out int aWritten, default, provider)) return false;
            charsWritten = aWritten;
            if (destination.Length <= charsWritten) return false;
            destination[charsWritten] = 'n';
            charsWritten++;
        }

        // 3. Handle B value
        if (B > 0)
        {
            // Append "+" to result, then append the serialization of B to result.
            if (destination.Length <= charsWritten) return false;
            destination[charsWritten] = '+';
            charsWritten++;
            if (!B.TryFormat(destination[charsWritten..], out int bWritten, default, provider)) return false;
            charsWritten += bWritten;
        }
        else if (B < 0)
        {
            // Append the serialization of B to result (includes the minus sign).
            if (!B.TryFormat(destination[charsWritten..], out int bWritten, default, provider)) return false;
            charsWritten += bWritten;
        }
        // If B == 0, append nothing

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[32];
        if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
        {
            return buffer[..charsWritten].ToString();
        }
        return string.Empty;
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is CssAnB other && A == other.A && B == other.B;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return System.HashCode.Combine(A, B);
    }

    public static bool operator ==(CssAnB left, CssAnB right) => left.Equals(right);
    public static bool operator !=(CssAnB left, CssAnB right) => !left.Equals(right);
}
