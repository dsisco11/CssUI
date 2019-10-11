using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CssUI
{
    /// <summary>
    /// A concurrently accessible HashSet, thread safe.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ConcurrentHashSet<T> : IReadOnlyCollection<T>, ISet<T>
    {
        public int Count => BackingCollection.Count;
        public bool IsReadOnly => false;

        private readonly ConcurrentDictionary<T, bool> BackingCollection = new ConcurrentDictionary<T, bool>();

        public bool Add(T item) => BackingCollection.TryAdd(item, true);
        public void Clear() => BackingCollection.Clear();
        public bool Contains(T item) => BackingCollection.ContainsKey(item);
        public void CopyTo(T[] array, int arrayIndex) => BackingCollection.Keys.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (T item in other)
            {
                Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator() => BackingCollection.Keys.GetEnumerator();

        public void IntersectWith(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();
            foreach (T item in this.Where(item => !collection.Contains(item)))
            {
                Remove(item);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();
            return (collection.Count != Count) && IsSubsetOf(collection);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();
            return (collection.Count != Count) && IsSupersetOf(collection);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();
            return this.AsParallel().All(collection.Contains);
        }

        public bool IsSupersetOf(IEnumerable<T> other) => other.AsParallel().All(Contains);
        public bool Overlaps(IEnumerable<T> other) => other.AsParallel().Any(Contains);
        public bool Remove(T item) => BackingCollection.TryRemove(item, out _);

        public bool SetEquals(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();
            return (collection.Count == Count) && collection.AsParallel().All(Contains);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ICollection<T> collection = other as ICollection<T> ?? other.ToList();

            HashSet<T> removed = new HashSet<T>();
            foreach (T item in collection.Where(Contains))
            {
                removed.Add(item);
                Remove(item);
            }

            foreach (T item in collection.Where(item => !removed.Contains(item)))
            {
                Add(item);
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (T otherElement in other)
            {
                Add(otherElement);
            }
        }

        void ICollection<T>.Add(T item) => Add(item);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // We use Count() and not Any() because we must ensure full loop pass
        internal bool AddRange(IEnumerable<T> items) => items.Any(Add);

        // We use Count() and not Any() because we must ensure full loop pass
        internal bool RemoveRange(IEnumerable<T> items) => items.Any(Remove);

        internal bool ReplaceIfNeededWith(ICollection<T> other)
        {
            if (SetEquals(other))
            {
                return false;
            }

            ReplaceWith(other);
            return true;
        }

        internal void ReplaceWith(IEnumerable<T> other)
        {
            BackingCollection.Clear();
            foreach (T item in other)
            {
                BackingCollection[item] = true;
            }
        }
    }
}
