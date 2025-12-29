using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CssUI.CSS;

/// <summary>
/// Manages multiple boolean values as single bits which allows for atomic operations and quick comparisons between large sets of boolean values.
/// Uses SIMD (Vector256) for hardware-accelerated bitwise operations when available.
/// </summary>
public sealed class FlagCollection<FlagType> : IEnumerable<FlagType> where FlagType : struct
{
    private readonly record struct FlagOffset(int Chunk, int Bit, uint Mask);
    private const int CHUNK_SIZE = sizeof(uint) * 8; // 32 bits per chunk
    private const int VECTOR_UINT_COUNT = 8; // Vector256<uint> holds 8 uints

    #region Properties
    /// <summary>
    /// Number of fields (bits) in this collection
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Number of chunks (uint values) in this collection
    /// </summary>
    private int ChunkCount { get; }

    /// <summary>
    /// The underlying chunk data storage (aligned for SIMD)
    /// </summary>
    private readonly uint[] _chunks;

    /// <summary>
    /// Number of flags currently set to an active (true) state
    /// </summary>
    public int ActiveFlags { get; private set; }

    /// <summary>
    /// Whether AVX2 SIMD operations are available on this hardware
    /// </summary>
    private static bool IsAvx2Supported => Avx2.IsSupported;
    #endregion

    #region Constructors
    public FlagCollection(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        Length = length;
        // Calculate number of uint chunks needed (each chunk holds 32 bits)
        // Pad to multiple of VECTOR_UINT_COUNT for SIMD alignment
        int minChunks = (length + CHUNK_SIZE - 1) / CHUNK_SIZE;
        ChunkCount = ((minChunks + VECTOR_UINT_COUNT - 1) / VECTOR_UINT_COUNT) * VECTOR_UINT_COUNT;

        // Array is automatically zero-initialized
        _chunks = GC.AllocateArray<uint>(ChunkCount, pinned: false);
    }

    /// <summary>
    /// Private constructor for creating copies with existing data
    /// </summary>
    private FlagCollection(int length, int chunkCount, ReadOnlySpan<uint> sourceChunks)
    {
        Length = length;
        ChunkCount = chunkCount;
        _chunks = GC.AllocateArray<uint>(ChunkCount, pinned: false);
        sourceChunks.CopyTo(_chunks);
        ActiveFlags = TallyActiveSimd();
    }
    #endregion

    #region Flag Accessors
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FlagOffset GetFlagOffset(int flagNum)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(flagNum);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(flagNum, Length);

        int bitNum = flagNum % CHUNK_SIZE;
        return new FlagOffset(
            Chunk: flagNum / CHUNK_SIZE,
            Bit: bitNum,
            Mask: 1u << bitNum
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetFlag(int flagNum)
    {
        var pos = GetFlagOffset(flagNum);
        return (_chunks[pos.Chunk] & pos.Mask) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool GetFlag(FlagType flag) => GetFlag(CastTo<int>.From(flag));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFlag(int flagNum)
    {
        var pos = GetFlagOffset(flagNum);
        ref uint chunk = ref _chunks[pos.Chunk];
        if ((chunk & pos.Mask) == 0)
        {
            ActiveFlags++;
            chunk |= pos.Mask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFlag(FlagType flag) => SetFlag(CastTo<int>.From(flag));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFlag(int flagNum, bool state)
    {
        var pos = GetFlagOffset(flagNum);
        ref uint chunk = ref _chunks[pos.Chunk];
        bool current = (chunk & pos.Mask) != 0;
        if (current != state)
        {
            ActiveFlags += state ? 1 : -1;
            if (state)
                chunk |= pos.Mask;
            else
                chunk &= ~pos.Mask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFlag(FlagType flag, bool state) => SetFlag(CastTo<int>.From(flag), state);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearFlag(int flagNum)
    {
        var pos = GetFlagOffset(flagNum);
        ref uint chunk = ref _chunks[pos.Chunk];
        if ((chunk & pos.Mask) != 0)
        {
            ActiveFlags--;
            chunk &= ~pos.Mask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearFlag(FlagType flag) => ClearFlag(CastTo<int>.From(flag));

    /// <summary>
    /// Clears all flags
    /// </summary>
    public void Clear()
    {
        if (ActiveFlags != 0)
        {
            ActiveFlags = 0;
            Array.Clear(_chunks);
        }
    }

    /// <summary>
    /// Tallies all set flags using SIMD PopCount when available
    /// </summary>
    private int TallyActiveSimd()
    {
        int count = 0;
        ReadOnlySpan<uint> chunks = _chunks;

        if (IsAvx2Supported && ChunkCount >= VECTOR_UINT_COUNT)
        {
            // Use SIMD for bulk processing
            ref uint chunksRef = ref MemoryMarshal.GetReference(chunks);
            int vectorizedLength = ChunkCount - (ChunkCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> vec = Vector256.LoadUnsafe(ref chunksRef, (nuint)i);
                // Sum popcount of each element
                for (int j = 0; j < VECTOR_UINT_COUNT; j++)
                {
                    count += System.Numerics.BitOperations.PopCount(vec.GetElement(j));
                }
            }

            // Handle remainder
            for (int i = vectorizedLength; i < ChunkCount; i++)
            {
                count += System.Numerics.BitOperations.PopCount(chunks[i]);
            }
        }
        else
        {
            // Scalar fallback
            foreach (uint chunk in chunks)
            {
                count += System.Numerics.BitOperations.PopCount(chunk);
            }
        }

        return count;
    }
    #endregion

    #region Value Tests
    /// <summary>
    /// Returns <c>True</c> if this collection has no set flags
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEmpty() => ActiveFlags == 0;

    public override bool Equals(object? obj)
    {
        if (obj is FlagCollection<FlagType> other && Length == other.Length)
        {
            return EqualsSimd(other);
        }
        return false;
    }

    private bool EqualsSimd(FlagCollection<FlagType> other)
    {
        if (IsAvx2Supported && ChunkCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(_chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(other._chunks);
            int vectorizedLength = ChunkCount - (ChunkCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> left = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> right = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                if (!left.Equals(right))
                    return false;
            }

            // Handle remainder
            for (int i = vectorizedLength; i < ChunkCount; i++)
            {
                if (_chunks[i] != other._chunks[i])
                    return false;
            }
            return true;
        }
        else
        {
            return _chunks.AsSpan().SequenceEqual(other._chunks);
        }
    }

    public static bool operator ==(FlagCollection<FlagType>? left, FlagCollection<FlagType>? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        if (left.Length != right.Length) return false;
        return left.EqualsSimd(right);
    }

    public static bool operator !=(FlagCollection<FlagType>? left, FlagCollection<FlagType>? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Returns <c>True</c> if the given collection also has all of this collection's active flags set
    /// </summary>
    public bool IsSubsetOf(FlagCollection<FlagType> right)
    {
        return (this & right) == this;
    }
    #endregion

    #region Bitwise Operators
    /// <summary>
    /// Returns the inverse of this collection
    /// </summary>
    public static FlagCollection<FlagType> operator ~(FlagCollection<FlagType> left)
    {
        var result = new FlagCollection<FlagType>(left.Length);

        if (IsAvx2Supported && left.ChunkCount >= VECTOR_UINT_COUNT)
        {
            ref uint srcRef = ref MemoryMarshal.GetArrayDataReference(left._chunks);
            ref uint dstRef = ref MemoryMarshal.GetArrayDataReference(result._chunks);
            int vectorizedLength = left.ChunkCount - (left.ChunkCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> vec = Vector256.LoadUnsafe(ref srcRef, (nuint)i);
                Vector256<uint> inverted = ~vec;
                inverted.StoreUnsafe(ref dstRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < left.ChunkCount; i++)
            {
                result._chunks[i] = ~left._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < left.ChunkCount; i++)
            {
                result._chunks[i] = ~left._chunks[i];
            }
        }

        result.ActiveFlags = left.Length - left.ActiveFlags;
        return result;
    }

    /// <summary>
    /// Returns a new collection with all flags that are in both this and the other collection
    /// </summary>
    public static FlagCollection<FlagType> operator &(FlagCollection<FlagType> left, FlagCollection<FlagType> right)
    {
        var result = new FlagCollection<FlagType>(left.Length);
        int minCount = Math.Min(left.ChunkCount, right.ChunkCount);

        if (IsAvx2Supported && minCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(left._chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(right._chunks);
            ref uint dstRef = ref MemoryMarshal.GetArrayDataReference(result._chunks);
            int vectorizedLength = minCount - (minCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> l = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> r = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                Vector256<uint> res = l & r;
                res.StoreUnsafe(ref dstRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] & right._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] & right._chunks[i];
            }
        }

        result.ActiveFlags = result.TallyActiveSimd();
        return result;
    }

    /// <summary>
    /// Returns a new collection with all flags that are in either this or the other collection
    /// </summary>
    public static FlagCollection<FlagType> operator |(FlagCollection<FlagType> left, FlagCollection<FlagType> right)
    {
        var result = new FlagCollection<FlagType>(left.Length);
        int minCount = Math.Min(left.ChunkCount, right.ChunkCount);

        if (IsAvx2Supported && minCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(left._chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(right._chunks);
            ref uint dstRef = ref MemoryMarshal.GetArrayDataReference(result._chunks);
            int vectorizedLength = minCount - (minCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> l = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> r = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                Vector256<uint> res = l | r;
                res.StoreUnsafe(ref dstRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] | right._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] | right._chunks[i];
            }
        }

        // Copy remaining from longer collection
        if (left.ChunkCount > minCount)
        {
            left._chunks.AsSpan(minCount).CopyTo(result._chunks.AsSpan(minCount));
        }

        result.ActiveFlags = result.TallyActiveSimd();
        return result;
    }

    /// <summary>
    /// Returns a new collection with all flags that are in either this or the other collection but not both
    /// </summary>
    public static FlagCollection<FlagType> operator ^(FlagCollection<FlagType> left, FlagCollection<FlagType> right)
    {
        var result = new FlagCollection<FlagType>(left.Length);
        int minCount = Math.Min(left.ChunkCount, right.ChunkCount);

        if (IsAvx2Supported && minCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(left._chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(right._chunks);
            ref uint dstRef = ref MemoryMarshal.GetArrayDataReference(result._chunks);
            int vectorizedLength = minCount - (minCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> l = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> r = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                Vector256<uint> res = l ^ r;
                res.StoreUnsafe(ref dstRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] ^ right._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < minCount; i++)
            {
                result._chunks[i] = left._chunks[i] ^ right._chunks[i];
            }
        }

        result.ActiveFlags = result.TallyActiveSimd();
        return result;
    }
    #endregion

    #region Bitwise Assignments

    /// <summary>
    /// Inverts this collection's flags
    /// </summary>
    public void Invert()
    {
        if (IsAvx2Supported && ChunkCount >= VECTOR_UINT_COUNT)
        {
            ref uint chunksRef = ref MemoryMarshal.GetArrayDataReference(_chunks);
            int vectorizedLength = ChunkCount - (ChunkCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> vec = Vector256.LoadUnsafe(ref chunksRef, (nuint)i);
                Vector256<uint> inverted = ~vec;
                inverted.StoreUnsafe(ref chunksRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < ChunkCount; i++)
            {
                _chunks[i] = ~_chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < ChunkCount; i++)
            {
                _chunks[i] = ~_chunks[i];
            }
        }

        ActiveFlags = Length - ActiveFlags;
    }

    /// <summary>
    /// Keeps only flags that are set in BOTH this collection and the given collection (intersection).
    /// Flags beyond the shorter collection's length are cleared.
    /// </summary>
    public void And(FlagCollection<FlagType> right)
    {
        int minCount = Math.Min(ChunkCount, right.ChunkCount);

        if (IsAvx2Supported && minCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(_chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(right._chunks);
            int vectorizedLength = minCount - (minCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> l = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> r = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                Vector256<uint> res = l & r;
                res.StoreUnsafe(ref leftRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < minCount; i++)
            {
                _chunks[i] &= right._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < minCount; i++)
            {
                _chunks[i] &= right._chunks[i];
            }
        }

        // Clear chunks beyond the right collection's size
        for (int i = minCount; i < ChunkCount; i++)
        {
            _chunks[i] = 0;
        }

        ActiveFlags = TallyActiveSimd();
    }

    /// <summary>
    /// Adds all of the set flags from the given collection to this one (union).
    /// Only processes flags up to the shorter of the two collections.
    /// </summary>
    public void Or(FlagCollection<FlagType> right)
    {
        int minCount = Math.Min(ChunkCount, right.ChunkCount);

        if (IsAvx2Supported && minCount >= VECTOR_UINT_COUNT)
        {
            ref uint leftRef = ref MemoryMarshal.GetArrayDataReference(_chunks);
            ref uint rightRef = ref MemoryMarshal.GetArrayDataReference(right._chunks);
            int vectorizedLength = minCount - (minCount % VECTOR_UINT_COUNT);

            for (int i = 0; i < vectorizedLength; i += VECTOR_UINT_COUNT)
            {
                Vector256<uint> l = Vector256.LoadUnsafe(ref leftRef, (nuint)i);
                Vector256<uint> r = Vector256.LoadUnsafe(ref rightRef, (nuint)i);
                Vector256<uint> res = l | r;
                res.StoreUnsafe(ref leftRef, (nuint)i);
            }

            for (int i = vectorizedLength; i < minCount; i++)
            {
                _chunks[i] |= right._chunks[i];
            }
        }
        else
        {
            for (int i = 0; i < minCount; i++)
            {
                _chunks[i] |= right._chunks[i];
            }
        }

        ActiveFlags = TallyActiveSimd();
    }
    #endregion

    #region Enumeration
    /// <summary>
    /// Returns all of the set flag indices
    /// </summary>
    public IEnumerator<FlagType> GetEnumerator()
    {
        if (ActiveFlags <= 0) yield break;

        int flagNum = 0;
        int actualChunkCount = (Length + CHUNK_SIZE - 1) / CHUNK_SIZE;

        for (int i = 0; i < actualChunkCount; i++)
        {
            uint chunkData = _chunks[i];

            // Skip empty chunks entirely
            if (chunkData == 0)
            {
                flagNum += CHUNK_SIZE;
                if (flagNum >= Length) yield break;
                continue;
            }

            // Use trailing zero count to find set bits quickly
            while (chunkData != 0)
            {
                int trailingZeros = System.Numerics.BitOperations.TrailingZeroCount(chunkData);
                int currentFlag = flagNum + trailingZeros;

                if (currentFlag >= Length) yield break;

                yield return CastTo<FlagType>.From(currentFlag);

                // Clear the lowest set bit
                chunkData &= chunkData - 1;
            }

            flagNum += CHUNK_SIZE;
            if (flagNum >= Length) yield break;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override int GetHashCode()
    {
        var hash = new HashCode();
        int actualChunkCount = (Length + CHUNK_SIZE - 1) / CHUNK_SIZE;
        for (int i = 0; i < actualChunkCount; i++)
        {
            hash.Add(_chunks[i]);
        }
        return hash.ToHashCode();
    }
    #endregion
}
