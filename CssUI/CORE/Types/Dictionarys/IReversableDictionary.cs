namespace CssUI
{
    public interface IReversableDictionary<TKey, TValue>
    {
        void Add(TKey key, TValue value);
        void Remove(TKey key, out TValue outValue);
        System.Boolean RemoveInverse(TValue value);
        System.Boolean RemoveInverse(TValue value, out TKey outKey);
        System.Boolean TryGetKey(TValue value, out TKey key);
        System.Boolean Update(TKey key, TValue newValue, TValue comparisonValue);
    }
}