using System;
using System.Diagnostics;

namespace CssUI
{
    /// <summary>
    /// Represents a globally unique identifier.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class UniqueID : IEquatable<UniqueID>
    {
        #region Proerties
        protected BitVector64 Bits;
        #endregion

        #region Accessors
        /// <summary>
        /// Gets or sets the entire 64bit value of this ID.
        /// </summary>
        public ulong Value
        {
            get { return Bits.Data; }
            set { Bits.Data = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueID"/> class.
        /// </summary>
        public UniqueID()
            : this(ulong.MaxValue)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueID"/> class.
        /// </summary>
        /// <param name="uid">The ID value.</param>
        public UniqueID(ulong uid)
        {
            this.Bits = new BitVector64(uid);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Performs an implicit conversion from <see cref="UniqueID"/> to <see cref="System.UInt64"/>.
        /// </summary>
        /// <param name="uid">The id.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ulong(UniqueID uid)
        {
            if (uid == null)
            {
                throw new ArgumentNullException(nameof(uid));
            }

            return uid.Bits.Data;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.UInt64"/> to <see cref="UniqueID"/>.
        /// </summary>
        /// <param name="uid">The id.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UniqueID(ulong uid)
        {
            return new UniqueID(uid);
        }
        #endregion
        

        #region Equality
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            UniqueID uid = obj as UniqueID;
            if ((object)uid == null)
            {
                return false;
            }

            return Bits.Data == uid.Bits.Data;
        }

        /// <summary>
        /// Determines whether the specified <see cref="UniqueID"/> is equal to this instance.
        /// </summary>
        /// <param name="uid">The <see cref="UniqueID"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="UniqueID"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(UniqueID uid)
        {
            if ((object)uid == null)
            {
                return false;
            }

            return Bits.Data == uid.Bits.Data;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">The left side ID.</param>
        /// <param name="b">The right side ID.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(UniqueID a, UniqueID b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Bits.Data == b.Bits.Data;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">The left side ID.</param>
        /// <param name="b">The right side ID.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(UniqueID a, UniqueID b)
        {
            return !(a == b);
        }
        #endregion
        

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Bits.Data.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value.ToString();
        }

    }
}
