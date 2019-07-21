using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an attribute value for a DOM <see cref="Element"/>
    /// These are not mutable, they cannot be changed after creation
    /// </summary>
    public class AttributeValue
    {
        #region Properties
        /// <summary>
        /// Name of this attribute
        /// </summary>
        //public readonly AtomicName<EAttributeName> Name;
        //private readonly WeakReference<AttributeDefinition> _definition;

        public readonly EAttributeType Type;
        /// <summary>
        /// The true string value
        /// </summary>
        public readonly string Data;
        /// <summary>
        /// The typed form of this value
        /// </summary>
        private readonly dynamic Value;
        #endregion

        #region Accessors
        #endregion

        #region Constructors
        private AttributeValue(EAttributeType Type, string Data)
        {
            this.Type = Type;
            this.Data = Data;
        }

        public AttributeValue(EAttributeType Type, string Data, dynamic Value) : this(Type, Data)
        {
            this.Value = Value;
        }
        #endregion

        #region Instantiators

        /// <summary>
        /// Creates a new <see cref="EAttributeType.Integer"/> type attribute value
        /// </summary>
        public static AttributeValue From_Integer(long Integer) => new AttributeValue(EAttributeType.Integer, Integer.ToString(), Integer);

        /// <summary>
        /// Creates a new <see cref="EAttributeType.FloatingPoint"/> type attribute value
        /// </summary>
        public static AttributeValue From_FloatingPoint(double Single) => new AttributeValue(EAttributeType.FloatingPoint, Single.ToString(), Single);

        /// <summary>
        /// Creates a new <see cref="EAttributeType.Length"/> type attribute value
        /// </summary>
        public static AttributeValue From_Length(double Length) => new AttributeValue(EAttributeType.Length, Length.ToString(), Length);
        
        /// <summary>
        /// Creates a new <see cref="EAttributeType.Percentage"/> type attribute value
        /// </summary>
        public static AttributeValue From_Percent(double Percentage) => new AttributeValue(EAttributeType.Percentage, Percentage.ToString(), Percentage);
        
        /// <summary>
        /// Creates a new <see cref="EAttributeType.Boolean"/> type attribute value
        /// </summary>
        public static AttributeValue From_Boolean(bool boolVal) => new AttributeValue(EAttributeType.Boolean, boolVal ? string.Empty : null, boolVal);

        /// <summary>
        /// Creates a new <see cref="EAttributeType.String"/> type attribute value
        /// </summary>
        public static AttributeValue From_String(string str) => new AttributeValue(EAttributeType.String, str, str);

        /// <summary>
        /// Creates a new <see cref="EAttributeType.Enumerated"/> type attribute value
        /// </summary>
        /// <typeparam name="Ty"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static AttributeValue From_Enum<Ty>(Ty enumValue) where Ty: struct
        {
            DomLookup.Keyword_From_Enum(enumValue, out string keyword);
            return new AttributeValue(EAttributeType.Enumerated, keyword, enumValue);
        }
        #endregion

        #region Parsing
        public static AttributeValue Parse(string Input, AttributeDefinition Def) 
        {
            Def.Parse(Input, out dynamic outVal);
            return new AttributeValue(Def.Type, Input, outVal);
        }
        #endregion

        #region Value Retreival
        /// <summary>
        /// Retreives this value as a string if possible
        /// </summary>
        public string Get_String() => Value as string;

        /// <summary>
        /// Retreives the RAW backing value
        /// </summary>
        public dynamic Get_RAW() => Value;

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public int Get_Int()
        {
            if (Type != EAttributeType.Integer)
            {
                throw new System.Exception("Cannot retrieve integer value for attribute whose type is not specified as such!");
            }

            return (int)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public uint Get_UInt()
        {
            if (Type != EAttributeType.NonNegative_Integer)
            {
                throw new System.Exception("Cannot retrieve unsigned-integer value for attribute whose type is not specified as such!");
            }

            return (uint)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public long Get_Long()
        {
            if (Type != EAttributeType.Integer)
            {
                throw new System.Exception("Cannot retrieve integer value for attribute whose type is not specified as such!");
            }

            return (long)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public ulong Get_ULong()
        {
            if (Type != EAttributeType.Integer)
            {
                throw new System.Exception("Cannot retrieve integer value for attribute whose type is not specified as such!");
            }

            return (uint)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public double Get_FloatingPoint()
        {
            if (Type != EAttributeType.FloatingPoint)
            {
                throw new System.Exception("Cannot retrieve floating-point value for attribute whose type is not specified as such!");
            }

            return (double)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public ulong Get_Enum<Ty>()
        {
            if (Type != EAttributeType.Enumerated)
            {
                throw new System.Exception("Cannot retrieve enum value for attribute whose type is not specified as such!");
            }

            return CastTo<Ty>.From(Value);
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public double Get_Length()
        {
            if (Type != EAttributeType.Length && Type != EAttributeType.NonZero_Length)
            {
                throw new System.Exception("Cannot retrieve length value for attribute whose type is not specified as such!");
            }

            return (double)Value;
        }

        /// <summary>
        /// Retreives this value as the requested type if possible
        /// </summary>
        public double Get_Percentage()
        {
            if (Type != EAttributeType.Percentage && Type != EAttributeType.NonZero_Percentage)
            {
                throw new System.Exception("Cannot retrieve percentage value for attribute whose type is not specified as such!");
            }

            return (double)Value;
        }


        #endregion

    }
}
