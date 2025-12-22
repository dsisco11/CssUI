using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using CssUI.DOM.Enums;

namespace CssUI.DOM;

/// <summary>
/// Represents an attribute value for a DOM <see cref="Element"/>
/// These are not mutable, they cannot be changed after creation
/// </summary>
public class AttributeValue
{
    #region Constants
    public static readonly AttributeValue NegativeOne = new AttributeValue(EAttributeType.Integer, -1, "-1");
    public static readonly AttributeValue Zero = new AttributeValue(EAttributeType.NonNegative_Integer, 0, "0");
    public static readonly AttributeValue One = new AttributeValue(EAttributeType.NonNegative_Integer, 1, "1");
    #endregion

    #region Properties
    /// <summary>
    /// Name of this attribute
    /// </summary>
    //public readonly AtomicName<EAttributeName> Name;
    //private readonly WeakReference<AttributeDefinition> _definition;

    private readonly EAttributeType type;
    /// <summary>
    /// The true string value
    /// </summary>
    private readonly string data;
    /// <summary>
    /// The typed form of this value
    /// </summary>
    private readonly object? Value;
    #endregion

    #region Accessors
    public EAttributeType Type => type;
    public string Data => data;
    #endregion

    #region Constructors
    private AttributeValue(EAttributeType Type, string Data)
    {
        type = Type;
        data = Data;
    }

    public AttributeValue(EAttributeType Type, object Value, string Data) : this(Type, Data.ToString(CultureInfo.InvariantCulture))
    {
        ArgumentNullException.ThrowIfNull(Data);
        ArgumentNullException.ThrowIfNull(Value);
        Contract.EndContractBlock();

        this.Value = Value;
    }
    #endregion

    #region Instantiators

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Integer"/> type attribute value
    /// </summary>
    public static AttributeValue From(Int32 Integer) => new AttributeValue(EAttributeType.Integer, Integer, Integer.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Integer"/> type attribute value
    /// </summary>
    public static AttributeValue From(UInt32 Integer) => new AttributeValue(EAttributeType.NonNegative_Integer, Integer, Integer.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Integer"/> type attribute value
    /// </summary>
    public static AttributeValue From(Int64 Integer) => new AttributeValue(EAttributeType.Integer, Integer, Integer.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Integer"/> type attribute value
    /// </summary>
    public static AttributeValue From(UInt64 Integer) => new AttributeValue(EAttributeType.NonNegative_Integer, Integer, Integer.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.FloatingPoint"/> type attribute value
    /// </summary>
    public static AttributeValue From(Double Single) => new AttributeValue(EAttributeType.FloatingPoint, Single, Single.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Boolean"/> type attribute value
    /// </summary>
    public static AttributeValue From(Boolean boolVal) => new AttributeValue(EAttributeType.Boolean, boolVal, boolVal ? string.Empty : string.Empty);

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Length"/> type attribute value
    /// </summary>
    public static AttributeValue From_Length(Double Length) => new AttributeValue(EAttributeType.Length, Length, Length.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Percentage"/> type attribute value
    /// </summary>
    public static AttributeValue From_Percent(Double Percentage) => new AttributeValue(EAttributeType.Percentage, Percentage, Percentage.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Creates a new <see cref="EAttributeType.String"/> type attribute value
    /// </summary>
    public static AttributeValue From(AtomicString str) => new AttributeValue(EAttributeType.String, str, str);

    /// <summary>
    /// Creates a new <see cref="EAttributeType.String"/> type attribute value from a plain string
    /// </summary>
    public static AttributeValue From(string str) => new AttributeValue(EAttributeType.String, new AtomicString(str), str);

    /// <summary>
    /// Creates a new <see cref="EAttributeType.Enumerated"/> type attribute value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static AttributeValue From<T>(T enumValue) where T : struct
    {
        /* Not all enumeration values will have a DOM keyword, some defined by the specification explicitly say certain values should NOT have a keyword */
        string keyword = string.Empty;

        if (Lookup.TryKeyword(enumValue, out string? outKey) && outKey != null)
            keyword = outKey;

        return new AttributeValue(EAttributeType.Enumerated, enumValue!, keyword);
    }
    #endregion

    #region Parsing
    public static AttributeValue Parse(AtomicName<EAttributeName> Name, string Input)
    {
        var Def = AttributeDefinition.Lookup(Name);
        if (Def is null)
        {
            // For custom attributes (data-* or unknown), treat as string
            return new AttributeValue(EAttributeType.String, new AtomicString(Input), Input);
        }
        Def.Parse(Input, out dynamic outVal);
        return new AttributeValue(Def.Type, outVal, Input);
    }
    #endregion

    #region Value Retreival
    /// <summary>
    /// Retreives this value as an <see cref="AtomicString"/> if possible
    /// </summary>
    public AtomicString? AsAtomic() => Value as AtomicString;

    /// <summary>
    /// Retreives this value as a string if possible
    /// </summary>
    public string? AsString()
    {
        if (Value is AtomicString atomicStr)
            return atomicStr.ToString();
        return Value as string;
    }

    /// <summary>
    /// Retreives the RAW backing value
    /// </summary>
    public object? AsRAW() => Value;

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public Int32 AsInt()
    {
        if (Type != EAttributeType.Integer)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (Int32)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public UInt32 AsUInt()
    {
        if (Type != EAttributeType.NonNegative_Integer)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (UInt32)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public Int64 AsLong()
    {
        if (Type != EAttributeType.Integer)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (Int64)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public UInt64 AsULong()
    {
        if (Type != EAttributeType.Integer)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (UInt64)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public Double AsFloatingPoint()
    {
        if (Type != EAttributeType.FloatingPoint)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (double)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public T AsEnum<T>()
    {
        if (Type != EAttributeType.Enumerated)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return CastTo<T>.From(Value);
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public Double AsLength()
    {
        if (Type != EAttributeType.Length && Type != EAttributeType.NonZero_Length)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (double)Value;
    }

    /// <summary>
    /// Retreives this value as the requested type if possible
    /// </summary>
    public Double AsPercentage()
    {
        if (Type != EAttributeType.Percentage && Type != EAttributeType.NonZero_Percentage)
        {
            throw new Exception(ExceptionMessages.ERROR_Attribute_Type_Mismatch);
        }

        return (double)Value;
    }


    #endregion

    #region Equality
    public override bool Equals(object? obj)
    {
        if (obj is not AttributeValue other)
            return false;

        if (Type == other.Type)
        {
            switch (Type)
            {
                case EAttributeType.Boolean:
                    {// A boolean attributes value is based on it's mere existance. So if the other object exists then they are both true and equal!
                        return other.Value is not null;
                    }
                case EAttributeType.Integer:
                    {// cast to large type just incase
                        return (Int64)Value == (Int64)other.Value;
                    }
                case EAttributeType.NonNegative_Integer:
                    {// cast to large type just incase
                        return (UInt64)Value == (UInt64)other.Value;
                    }
                case EAttributeType.Enumerated:
                    {
                        return (int)Value == (int)other.Value;
                    }
                case EAttributeType.NonZero_Length:
                case EAttributeType.NonZero_Percentage:
                case EAttributeType.FloatingPoint:
                    {
                        return ((double)Value == (double)other.Value);
                    }
                default:
                    {
                        if (other.Type == EAttributeType.String)
                        {
                            // Check if both values are AtomicStrings before comparing
                            if (Value is AtomicString atomicValue && other.Value is AtomicString otherAtomicValue)
                            {
                                return atomicValue.Equals(otherAtomicValue);
                            }
                            // Fall back to string comparison
                            return StringCommon.StrEq(Data, other.Data);
                        }
                        else
                        {
                            return StringCommon.StrEq(Data, other.Data);
                        }
                    }
            }
        }
        else
        {/* Its possible these values are compatible but have different types */
            return StringCommon.StrEq(Data, other.Data);
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion

}

