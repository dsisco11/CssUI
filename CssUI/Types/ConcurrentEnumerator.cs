using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace CssUI
{
    public sealed class ConcurrentEnumerator<T> : IEnumerator<T>
    {
        public T Current => Enumerator.Current;

        private readonly IEnumerator<T> Enumerator;
        private readonly SemaphoreSlim SemaphoreSlim;

        object IEnumerator.Current => Current;

        internal ConcurrentEnumerator(ICollection<T> collection, SemaphoreSlim semaphoreSlim)
        {
            if ((collection == null) || (semaphoreSlim == null))
            {
                throw new ArgumentNullException(nameof(collection) + " || " + nameof(semaphoreSlim));
            }

            SemaphoreSlim = semaphoreSlim;
            SemaphoreSlim.Wait();

            Enumerator = collection.GetEnumerator();
        }

        public void Dispose() => SemaphoreSlim.Release();
        public bool MoveNext() => Enumerator.MoveNext();
        public void Reset() => Enumerator.Reset();
    }
}
