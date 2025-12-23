using System;

namespace CssUI.CSS.Parser;

public sealed class DimensionToken : ValuedTokenBase
{
    /// <summary>
    /// Holds the numeric representation of this token value without boxing.
    /// </summary>
    private readonly NumericTokenData data;

    /// <summary>
    /// Specifies the type of value stored (integer or number/float).
    /// </summary>
    public readonly ENumericTokenType DataType = ENumericTokenType.Number;

    /// <summary>
    /// Holds the dimension's unit type string.
    /// </summary>
    public readonly string Unit;

    /// <summary>
    /// Gets the numeric value as an object (for backward compatibility).
    /// This boxes the value - prefer using <see cref="AsInteger"/> or <see cref="AsNumber"/> instead.
    /// </summary>
    public object Number => DataType == ENumericTokenType.Integer
        ? data.IntegerValue
        : data.NumberValue;

    /// <summary>
    /// Gets the value as an integer. Only valid when <see cref="DataType"/> is <see cref="ENumericTokenType.Integer"/>.
    /// </summary>
    public int AsInteger => data.IntegerValue;

    /// <summary>
    /// Gets the value as a double. Works for both integer and number types.
    /// </summary>
    public double AsNumber => DataType == ENumericTokenType.Integer
        ? data.IntegerValue
        : data.NumberValue;

    public DimensionToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, int number, ReadOnlySpan<char> Unit) : base(ECssTokenType.Dimension, Value)
    {
        this.DataType = DataType;
        this.data = NumericTokenData.FromInteger(number);
        this.Unit = Unit.ToString();
    }

    public DimensionToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, double number, ReadOnlySpan<char> Unit) : base(ECssTokenType.Dimension, Value)
    {
        this.DataType = DataType;
        this.data = NumericTokenData.FromNumber(number);
        this.Unit = Unit.ToString();
    }

    /// <summary>
    /// Legacy constructor for backward compatibility. Prefer the typed constructors.
    /// </summary>
    public DimensionToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, object number, ReadOnlySpan<char> Unit) : base(ECssTokenType.Dimension, Value)
    {
        this.DataType = DataType;
        if (DataType == ENumericTokenType.Integer && number is int intValue)
        {
            this.data = NumericTokenData.FromInteger(intValue);
        }
        else
        {
            this.data = NumericTokenData.FromNumber(Convert.ToDouble(number));
        }
        this.Unit = Unit.ToString();
    }

    public override string Encode()
    {
        return string.Concat(Value, Unit);
    }


    #region Equality Operators
    public override bool Equals(object? o)
    {
        if (o is DimensionToken Other)
        {
            return Type == Other.Type && Unit.Equals(Other.Unit, StringComparison.OrdinalIgnoreCase) && Value.Equals(Other.Value, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Unit, Value);
    }
    #endregion
}
