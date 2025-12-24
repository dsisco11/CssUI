using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CssUI;

/// <summary>
/// A <see cref="DataConsumer{T}"/> that transparently skips items matching a filter predicate.
/// Provides efficient filtered iteration without copying data.
/// </summary>
/// <remarks>
/// This consumer maintains the same underlying data but automatically skips items
/// that match the provided predicate when peeking, consuming, or checking stream state.
/// The filtered offset (e.g., Peek(2)) counts only non-filtered items.
/// </remarks>
/// <typeparam name="T">The type of items in the stream.</typeparam>
public sealed class FilteringDataConsumer<T> : DataConsumer<T>
{
    #region Fields
    /// <summary>
    /// Predicate that returns true for items that should be skipped.
    /// </summary>
    private readonly Predicate<T> _skipPredicate;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new filtering stream from a memory pointer.
    /// </summary>
    /// <param name="memory">The data to consume.</param>
    /// <param name="skipPredicate">Predicate returning true for items to skip.</param>
    public FilteringDataConsumer(ReadOnlyMemory<T> memory, Predicate<T> skipPredicate)
        : base(memory)
    {
        ArgumentNullException.ThrowIfNull(skipPredicate);
        Contract.EndContractBlock();

        _skipPredicate = skipPredicate;
    }

    /// <summary>
    /// Creates a new filtering stream from a memory pointer with EOF item.
    /// </summary>
    /// <param name="memory">The data to consume.</param>
    /// <param name="skipPredicate">Predicate returning true for items to skip.</param>
    /// <param name="eofItem">The item representing end-of-file.</param>
    public FilteringDataConsumer(ReadOnlyMemory<T> memory, Predicate<T> skipPredicate, T eofItem)
        : base(memory, eofItem)
    {
        ArgumentNullException.ThrowIfNull(skipPredicate);
        Contract.EndContractBlock();

        _skipPredicate = skipPredicate;
    }

    /// <summary>
    /// Creates a new filtering stream from an array.
    /// </summary>
    /// <param name="items">The items to consume.</param>
    /// <param name="skipPredicate">Predicate returning true for items to skip.</param>
    public FilteringDataConsumer(T[] items, Predicate<T> skipPredicate)
        : base(items)
    {
        ArgumentNullException.ThrowIfNull(skipPredicate);
        Contract.EndContractBlock();

        _skipPredicate = skipPredicate;
    }

    /// <summary>
    /// Creates a new filtering stream from an array with EOF item.
    /// </summary>
    /// <param name="items">The items to consume.</param>
    /// <param name="skipPredicate">Predicate returning true for items to skip.</param>
    /// <param name="eofItem">The item representing end-of-file.</param>
    public FilteringDataConsumer(T[] items, Predicate<T> skipPredicate, T eofItem)
        : base(items, eofItem)
    {
        ArgumentNullException.ThrowIfNull(skipPredicate);
        Contract.EndContractBlock();

        _skipPredicate = skipPredicate;
    }
    #endregion

    #region Filtered Accessors
    /// <summary>
    /// Returns the next non-filtered item to be consumed.
    /// </summary>
    public new T Next => PeekFiltered(0);

    /// <summary>
    /// Returns the second non-filtered item to be consumed.
    /// </summary>
    public new T NextNext => PeekFiltered(1);

    /// <summary>
    /// Returns the third non-filtered item to be consumed.
    /// </summary>
    public new T NextNextNext => PeekFiltered(2);

    /// <summary>
    /// Returns whether the stream has no more non-filtered items remaining.
    /// </summary>
    public new bool atEnd => IsFilteredEnd();

    /// <summary>
    /// Returns whether the next non-filtered item is the EOF item.
    /// </summary>
    public new bool atEOF => object.Equals(PeekFiltered(0), EOF_ITEM);

    /// <summary>
    /// Returns the count of remaining non-filtered items from current position.
    /// Note: This is O(n) as it must scan the remaining items.
    /// </summary>
    public int FilteredRemaining
    {
        get
        {
            int count = 0;
            var span = AsSpan();
            for (int i = Position; i < span.Length; i++)
            {
                if (!_skipPredicate(span[i]))
                {
                    count++;
                }
            }
            return count;
        }
    }
    #endregion

    #region Filtered Peeking
    /// <summary>
    /// Returns the item at the specified filtered offset from current position.
    /// The offset counts only non-filtered items.
    /// </summary>
    /// <param name="filteredOffset">Number of non-filtered items to skip (0 = next non-filtered item).</param>
    /// <returns>The item at the filtered offset, or EOF_ITEM if past end.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PeekFiltered(int filteredOffset)
    {
        if (filteredOffset < 0)
        {
            throw new IndexOutOfRangeException($"{nameof(filteredOffset)}({filteredOffset}) cannot be negative");
        }

        var span = AsSpan();
        int skipped = 0;
        int index = Position;

        while (index < span.Length)
        {
            var item = span[index];

            // Skip filtered items
            if (_skipPredicate(item))
            {
                index++;
                continue;
            }

            // Found a non-filtered item
            if (skipped == filteredOffset)
            {
                return item;
            }

            skipped++;
            index++;
        }

        return EOF_ITEM;
    }

    /// <summary>
    /// Peeks at the specified filtered offset.
    /// </summary>
    public new T Peek(int offset = 0) => PeekFiltered(offset);

    /// <summary>
    /// Peeks at the specified filtered offset.
    /// </summary>
    public new T Peek(uint offset) => PeekFiltered((int)offset);

    /// <summary>
    /// Peeks at the specified filtered offset.
    /// </summary>
    public new T Peek(long offset) => PeekFiltered((int)offset);

    /// <summary>
    /// Peeks at the specified filtered offset.
    /// </summary>
    public new T Peek(ulong offset) => PeekFiltered((int)offset);
    #endregion

    #region Filtered Consumption
    /// <summary>
    /// Consumes and returns the next non-filtered item, advancing past any filtered items.
    /// </summary>
    /// <returns>The next non-filtered item, or EOF_ITEM if at end.</returns>
    public new T Consume()
    {
        SkipFiltered();

        if (base.atEnd)
        {
            return EOF_ITEM;
        }

        return base.Consume();
    }

    /// <summary>
    /// Consumes and returns the next non-filtered item cast to the specified type.
    /// </summary>
    public new TResult Consume<TResult>() where TResult : T
    {
        SkipFiltered();

        if (base.atEnd)
        {
            return default!;
        }

        return base.Consume<TResult>();
    }

    /// <summary>
    /// Advances the stream position past any items matching the skip predicate.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipFiltered()
    {
        while (!base.atEnd && _skipPredicate(base.Next))
        {
            base.Consume();
        }
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Checks if there are no more non-filtered items remaining.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsFilteredEnd()
    {
        var span = AsSpan();
        for (int i = Position; i < span.Length; i++)
        {
            if (!_skipPredicate(span[i]))
            {
                return false;
            }
        }
        return true;
    }
    #endregion
}
