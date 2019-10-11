using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace CssUI.CSS
{
    /// <summary>
    /// Manages multiple boolean values as single bits which allows for atomic operations and quick comparisons between large sets of boolean values
    /// </summary>
    public class FlagCollection<FlagType> : IDisposable, IEnumerable<FlagType> where FlagType : struct
    {
        private struct FlagOffset { public int Chunk, Bit; public uint Mask; }
        const int CHUNK_SIZE = sizeof(int)*8;

        #region Properties
        /// <summary>
        /// Number of fields (bits) in this collection
        /// </summary>
        public readonly int Length;
        /// <summary>
        /// Size in bytes of collection
        /// </summary>
        private readonly uint Size;
        private unsafe uint* ChunkData;
        /// <summary>
        /// Number of flags currently set to an active(true) state
        /// </summary>
        public int ActiveFlags { get; private set; }
        #endregion

        #region Constructors
        public FlagCollection(int length)
        {
            Length = length;
            Size = (uint)Math.Ceiling((double)Length / CHUNK_SIZE);
            unsafe
            {
                ChunkData = (uint*)Marshal.AllocHGlobal((int)Size);
            }
        }

        #region IDisposable Support
        private int Disposed = 0;

        protected virtual void Dispose(bool userInitiated)
        {
            if (Interlocked.Exchange(ref Disposed, 1) == 0)
            {
                if (userInitiated)
                {
                    unsafe
                    {
                        Marshal.FreeHGlobal((IntPtr)ChunkData);
                        ChunkData = null;
                    }
                }
            }
        }

        ~FlagCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
             GC.SuppressFinalize(this);
        }
        #endregion
        #endregion

        #region Flag Accessors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint Get_Chunk_Data(int Chunk)
        {
            unsafe
            {
                return ChunkData[Chunk];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private FlagOffset Get_Flag_Offset(int FlagNum)
        {
            Contract.Requires<ArgumentOutOfRangeException>(FlagNum < Length, nameof(FlagNum));
            var BitNum = FlagNum % CHUNK_SIZE;
            return new FlagOffset()
            {
                Chunk = FlagNum / CHUNK_SIZE,
                Bit = BitNum,
                Mask = 1u << BitNum
            };
        }

        public bool GetFlag(int FlagNum)
        {
            var pos = Get_Flag_Offset(FlagNum);
            unsafe
            {
                return (ChunkData[pos.Chunk] & pos.Mask) != 0;
            }
        }

        public void SetFlag(int FlagNum)
        {
            var pos = Get_Flag_Offset(FlagNum);
            unsafe
            {
                if ((ChunkData[pos.Chunk] & pos.Mask) == 0)
                {
                    ActiveFlags += 1;
                    ChunkData[pos.Chunk] |= pos.Mask;
                }
            }
        }

        public void SetFlag(int FlagNum, bool State)
        {
            var pos = Get_Flag_Offset(FlagNum);
            unsafe
            {
                bool current = (ChunkData[pos.Chunk] & pos.Mask) != 0;
                if (current != State)
                {// Flag is going to change state
                    ActiveFlags += State ? 1 : -1;
                    ChunkData[pos.Chunk] = (ChunkData[pos.Chunk] & ~pos.Mask) | ((uint)(-(State ? 1 : 0)) & pos.Mask);
                }
            }
        }

        public void ClearFlag(int FlagNum)
        {
            var pos = Get_Flag_Offset(FlagNum);
            unsafe
            {
                if ((ChunkData[pos.Chunk] & pos.Mask) != 0)
                {
                    ActiveFlags -= 1;
                    ChunkData[pos.Chunk] &= ~pos.Mask;
                }
            }
        }

        /// <summary>
        /// Clears all flags
        /// </summary>
        public void Clear()
        {
            if (ActiveFlags != 0)
            {
                ActiveFlags = 0;
                unsafe
                {
                    Unsafe.InitBlock(((void*)ChunkData), 0x0, Size);
                }
            }
        }

        /// <summary>
        /// Manually tallys all of the set flags
        /// </summary>
        private int Tally_Active()
        {
            int RetVal = 0;
            for (int i = 0; i<Length; i++)
            {
                if (GetFlag(i))
                {
                    RetVal++;
                }
            }
            return RetVal;
        }
        #endregion

        #region Value Tests
        /// <summary>
        /// Returns <c>True</c> if this collection has no set flags
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return ActiveFlags == 0;
            unsafe
            {
                for (int i = 0; i < Size; i++)
                {
                    if (ChunkData[i] != 0)
                        return false;
                }
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is FlagCollection<FlagType> other)
            {
                if (Size == other.Size)
                {
                    unsafe
                    {
                        for (int i=0; i<Size; i++)
                        {
                            if (ChunkData[i] != other.ChunkData[i])
                                return false;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool operator ==(FlagCollection<FlagType> Left, FlagCollection<FlagType> Right)
        {
            if (Left.Size == Right.Size)
            {
                unsafe
                {
                    for (int i = 0; i < Left.Size; i++)
                    {
                        if (Left.ChunkData[i] != Right.ChunkData[i])
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(FlagCollection<FlagType> Left, FlagCollection<FlagType> Right)
        {
            return !(Left == Right);
        }

        /// <summary>
        /// Returns <c>True</c> if the given collection also has all of this collections active flags set
        /// </summary>
        public bool IsSubsetOf(FlagCollection<FlagType> Right)
        {
            return (this & Right) == this;
        }
        #endregion

        #region Bitwise Operators
        /// <summary>
        /// Returns the inverse of this collection
        /// </summary>
        public static FlagCollection<FlagType> operator ~(FlagCollection<FlagType> Left)
        {
            var RetVal = new FlagCollection<FlagType>(Left.Length);
            unsafe
            {
                for (int i = 0; i < Left.Size; i++)
                {
                    RetVal.ChunkData[i] = ~Left.ChunkData[i];
                }
            }
            return RetVal;
        }

        /// <summary>
        /// Returns a new collection with all flags that are in both this and the other collection
        /// </summary>
        public static FlagCollection<FlagType> operator &(FlagCollection<FlagType> Left, FlagCollection<FlagType> Right)
        {
            var RetVal = new FlagCollection<FlagType>(Left.Length);
            unsafe
            {
                for (int i=0; i<Left.Size; i++)
                {
                    RetVal.ChunkData[i] = Left.ChunkData[i] & Right.ChunkData[i];
                }
            }
            return RetVal;
        }

        /// <summary>
        /// Returns a new collection with all flags that are in either this or the other collection
        /// </summary>
        public static FlagCollection<FlagType> operator |(FlagCollection<FlagType> Left, FlagCollection<FlagType> Right)
        {
            var RetVal = new FlagCollection<FlagType>(Left.Length);
            unsafe
            {
                for (int i = 0; i < Left.Size; i++)
                {
                    RetVal.ChunkData[i] = Left.ChunkData[i] | Right.ChunkData[i];
                }
            }
            return RetVal;
        }

        /// <summary>
        /// Returns a new collection with all flags that are in either this or the other collection but not both
        /// </summary>
        public static FlagCollection<FlagType> operator ^(FlagCollection<FlagType> Left, FlagCollection<FlagType> Right)
        {
            var RetVal = new FlagCollection<FlagType>(Left.Length);
            unsafe
            {
                for (int i=0; i<Left.Size; i++)
                {
                    RetVal.ChunkData[i] = Left.ChunkData[i] ^ Right.ChunkData[i];
                }
            }
            return RetVal;
        }
        #endregion

        #region Bitwise Assignments

        /// <summary>
        /// Inverts this collections flags
        /// </summary>
        public void Invert()
        {
            unsafe
            {
                for (int i = 0; i < Size; i++)
                {
                    ChunkData[i] = ~ChunkData[i];
                }
                ActiveFlags = (Length - ActiveFlags);
            }
        }

        /// <summary>
        /// Adds all of the set flags from the given collection to this one
        /// </summary>
        public void And(FlagCollection<FlagType> Right)
        {
            unsafe
            {
                for (int i = 0; i < Size; i++)
                {
                    ChunkData[i] &= Right.ChunkData[i];
                }
                /// There's no smart way to determine this, we have to do it manually
                ActiveFlags = Tally_Active();
            }
        }


        /// <summary>
        /// Returns all of the set flags
        /// </summary>
        public IEnumerator<FlagType> GetEnumerator()
        {
            if (ActiveFlags <= 0) yield break;

            int FlagNum = 0;
            for (int i = 0; i < Size; i++)
            {
                uint mask = unchecked( (uint)-1 );
                for (int j = 0; j<CHUNK_SIZE; j++)
                {
                    mask <<= 1;
                    if((Get_Chunk_Data(i) & mask) != 0)
                    {
                        yield return CastTo<FlagType>.From(FlagNum);
                    }
                    FlagNum++;
                }
            }

            yield break;
        }

        /// <summary>
        /// Returns all of the set flags
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

    }
}
