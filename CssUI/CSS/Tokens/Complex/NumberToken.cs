using System;

namespace CssUI.CSS.Parser;

public sealed class NumberToken : ValuedTokenBase
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

    public NumberToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, int number) : base(ECssTokenType.Number, Value)
    {
        this.DataType = DataType;
        this.data = NumericTokenData.FromInteger(number);
    }

    public NumberToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, double number) : base(ECssTokenType.Number, Value)
    {
        this.DataType = DataType;
        this.data = NumericTokenData.FromNumber(number);
    }

    /// <summary>
    /// Legacy constructor for backward compatibility. Prefer the typed constructors.
    /// </summary>
    public NumberToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, object number) : base(ECssTokenType.Number, Value)
    {
        this.DataType = DataType;
        if (DataType == ENumericTokenType.Integer)
        {
            // Handle both int and long from tokenizer's ToInteger which returns long
            int intValue = number switch
            {
                int i => i,
                long l => (int)l,
                _ => Convert.ToInt32(number)
            };
            this.data = NumericTokenData.FromInteger(intValue);
        }
        else
        {
            this.data = NumericTokenData.FromNumber(Convert.ToDouble(number));
        }
    }


    public override bool Equals(object? o)
    {
        if (o is NumberToken Other)
        {
            return Type == Other.Type && DataType == Other.DataType && Value.Equals(Other.Value, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, DataType, Value);
    }
}

