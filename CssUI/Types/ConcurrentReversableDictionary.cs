using System;
using System.Collections.Concurrent;

namespace Vault
{
    public class ConcurrentReversableDictionary<Key, Value> : ConcurrentDictionary<Key, Value>
    {
        #region Properties
        ConcurrentDictionary<Value, Key> MapInverse = new ConcurrentDictionary<Value, Key>();
        #endregion

        #region Constructors
        public ConcurrentReversableDictionary()
        {
        }
        #endregion

        new public bool TryAdd(Key key, Value value)
        {
            if ( base.TryAdd(key, value) )
            {
                if ( !MapInverse.TryAdd(value, key) )
                {
                    throw new Exception($"Unable to add value-key entry from inverse dictionary! ValueType: {typeof(Value).FullName}");
                }
                return true;
            }

            return false;
        }

        new public bool TryRemove(Key key, out Value value)
        {
            if (base.TryRemove(key, out value))
            {
                if ( !MapInverse.TryRemove(value, out var _) )
                {
                    throw new Exception($"Unable to remove value-key entry from inverse dictionary! ValueType: {typeof(Value).FullName}");
                }
            }

            return false;
        }


        public bool TryRemoveInverse(Value value, out Key key)
        {
            if ( !MapInverse.TryRemove(value, out key) )
            {
                //throw new Exception($"Unable to remove value-key entry from inverse dictionary! ValueType: {typeof(Value).FullName}");
                return false;
            }

            if ( !base.TryRemove(key, out _) )
            {
                //throw new Exception($"Unable to remove key-value entry from dictionary! KeyType: {typeof(Key).FullName}");
                return false;
            }

            return true;
        }

        new public bool TryUpdate(Key key, Value newValue, Value comparisonValue)
        {
            throw new NotImplementedException();
        }

        public bool TryGetKey(Value value, out Key key) => MapInverse.TryGetValue(value, out key);

    }
}
