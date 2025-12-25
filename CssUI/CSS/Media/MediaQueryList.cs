using System;
using System.Buffers;
using System.Collections.Generic;
using CssUI.CSS.Media;
using CssUI.DOM.Events;

namespace CssUI.DOM.Media;

public class MediaQueryList : EventTarget, ISpanFormattable, IDisposable
{/* Docs: https://www.w3.org/TR/cssom-view-1/#mediaquerylist */
    #region Properties
    public readonly Document document;
    public readonly LinkedList<MediaQuery> QueryList;
    public readonly string media;
    private bool oldMatchState = false;
    #endregion

    #region Accessors
    /// <summary>
    /// Returns true if all of the media queries in this list match it's document.
    /// </summary>
    public bool Matches
    {
        get
        {
            foreach (MediaQuery query in QueryList)
            {
                if (!query.Matches(document))
                {
                    return false;
                }
            }

            return true;
        }
    }
    #endregion

    #region Events
    public event EventCallback onchange
    {
        add => handlerMap.Add(EEventName.Change, value);
        remove => handlerMap.Remove(EEventName.Change, value);
    }
    #endregion

    #region Constructors

    public MediaQueryList(Document document, LinkedList<MediaQuery> queryList)
    {
        this.document = document;
        this.QueryList = queryList;
        this.media = ToString();
        this.document._mediaQueryLists.AddLast(this);
    }
    #endregion


    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposedValue)
        {
            if (disposing)
            {
                this.document._mediaQueryLists.Remove(this);
            }
            disposedValue = true;
        }
    }
    #endregion

    internal void Evaluate()
    {/* Docs: https://www.w3.org/TR/cssom-view-1/#evaluate-media-queries-and-report-changes */
        bool newMatchState = Matches;
        if (newMatchState != oldMatchState)
        {
            oldMatchState = newMatchState;
            var evt = new MediaQueryListEvent(EEventName.Change, new MediaQueryListEventInit(media, newMatchState));
            dispatchEvent(evt);
        }
    }

    public void addEventListener(EventListener listener)
    {
        if (listener == null || listener.callback == null)
        {
            return;
        }

        base.addEventListener(EEventName.Change, listener.callback, new AddEventListenerOptions(false));
    }

    public void removeEventListener(EventListener listener)
    {
        if (listener?.callback == null)
        {
            return;
        }

        base.removeEventListener(EEventName.Change, listener.callback, new EventListenerOptions(false));
    }


    #region Formatting
    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        int written;
        bool first = true;

        foreach (MediaQuery query in QueryList)
        {
            if (!first)
            {
                if (!", ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 2;
            }

            if (!query.TryFormat(destination[charsWritten..], out written, format, provider))
                return false;
            charsWritten += written;
            first = false;
        }

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Estimate buffer size based on query count
        int estimatedSize = 64 + (QueryList.Count * 64);
        char[]? rented = null;
        Span<char> buffer = estimatedSize <= 512
            ? stackalloc char[512]
            : (rented = ArrayPool<char>.Shared.Rent(estimatedSize));

        try
        {
            if (TryFormat(buffer, out int charsWritten, default, formatProvider))
                return new string(buffer[..charsWritten]);

            // Fallback for unexpectedly large output
            rented = ArrayPool<char>.Shared.Rent(estimatedSize * 2);
            buffer = rented;
            if (TryFormat(buffer, out charsWritten, default, formatProvider))
                return new string(buffer[..charsWritten]);

            throw new InvalidOperationException("Buffer too small for MediaQueryList serialization");
        }
        finally
        {
            if (rented != null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);
    #endregion
}

