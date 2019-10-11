using xLog;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CssUI
{
    public class ConcurrentDoubleKeyDictionary<Key1, Key2, Vty>
    {
        #region Properties
        private ConcurrentDictionary<Key1, ConcurrentDictionary<Key2, Vty>> Map = new ConcurrentDictionary<Key1, ConcurrentDictionary<Key2, Vty>>();
        private ConcurrentDictionary<Key2, ConcurrentDictionary<Key1, Vty>> MapInverse = new ConcurrentDictionary<Key2, ConcurrentDictionary<Key1, Vty>>();
        #endregion

        #region Constructors
        public ConcurrentDoubleKeyDictionary()
        {
        }
        #endregion

        public Vty this[Key1 key1, Key2 key2]
        {
            get
            {
                if (Map.ContainsKey(key1) && MapInverse.ContainsKey(key2))
                    return Map[key1][key2];
                else
                    return default(Vty);
            }
            set
            {
                Map[key1][key2] = value;
                MapInverse[key2][key1] = value;
            }
        }

        #region TryAdd
        public bool TryAdd(Key1 key1, Key2 key2, Vty value)
        {
            if ( !Map.ContainsKey(key1) )
            {
                Map.TryAdd(key1, new ConcurrentDictionary<Key2, Vty>());
            }

            if ( !MapInverse.ContainsKey(key2) )
            {
                MapInverse.TryAdd(key2, new ConcurrentDictionary<Key1, Vty>());
            }

            if ( Map[key1].TryAdd(key2, value) )
            {
                if ( !MapInverse[key2].TryAdd(key1, value) )
                {
                    Log.Error(nameof(ConcurrentDoubleKeyDictionary<Key1, Key2, Vty>), $"Failed to add entry to inverse map!");
                }

                return true;
            }

            return false;
        }
        #endregion

        #region TryRemove
        public bool TryRemove(Key1 key1, Key2 key2)
        {
            if ( Map.ContainsKey(key1) )
            {
                if ( Map[key1].TryRemove(key2, out var subList) )
                {
                    if ( MapInverse.ContainsKey(key2) )
                    {
                        MapInverse[key2].TryRemove(key1, out var _);
                        if (MapInverse.Keys.Count <= 0)
                        {
                            MapInverse.TryRemove(key2, out var _);
                        }
                    }
                    return true;
                }

                if (Map[key1].Keys.Count <= 0)
                {
                    Map.TryRemove(key1, out var _);
                }
            }
            return false;
        }

        public bool TryRemove(Key1 key1)
        {
            if (Map.TryRemove(key1, out var subList))
            {// cleanup any key1 instances from all of its subkeys in the inverse map
                foreach (var kv in subList)
                {
                    Key2 key2 = kv.Key;
                    if (MapInverse.ContainsKey(key2))
                    {
                        MapInverse[key2].TryRemove(key1, out var _);
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryRemoveInverse(Key2 key2)
        {
            if (MapInverse.TryRemove(key2, out var subList))
            {// cleanup any key2 instances from all of its subkeys in the forward map
                foreach (var kv in subList)
                {
                    Key1 key1 = kv.Key;
                    if (Map.ContainsKey(key1))
                    {
                        Map[key1].TryRemove(key2, out var _);
                    }
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Contains
        public bool Contains(Key1 key1, Key2 key2)
        {
            if ( Map.ContainsKey( key1 ) )
            {
                if ( Map[key1].ContainsKey( key2 ) )
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Key1 key1)
        {
            return Map.ContainsKey(key1);
        }

        public bool ContainsInverse(Key2 key2)
        {
            return MapInverse.ContainsKey(key2);
        }
        #endregion

        #region Lookup
        public bool Lookup(Key1 key1, out ConcurrentDictionary<Key2, Vty> Value)
        {
            if (Map.TryGetValue(key1, out var v))
            {
                Value = v;
                return true;
            }

            Value = null;
            return false;
        }
        public bool LookupInverse(Key2 key2, out ConcurrentDictionary<Key1, Vty> Value)
        {
            if (MapInverse.TryGetValue(key2, out var v))
            {
                Value = v;
                return true;
            }

            Value = null;
            return false;
        }

        public bool Lookup(Key1 key1, Key2 key2, out Vty Value)
        {
            if ( Map.TryGetValue(key1, out var subMap) )
            {
                if ( subMap.TryGetValue(key2, out var v) )
                {
                    Value = v;
                    return true;
                }
            }

            Value = default(Vty);
            return false;
        }
        public bool LookupInverse(Key2 key2, Key1 key1, out Vty Value)
        {
            if (MapInverse.TryGetValue(key2, out var subMap))
            {
                if (subMap.TryGetValue(key1, out var v))
                {
                    Value = v;
                    return true;
                }
            }

            Value = default(Vty);
            return false;
        }
        #endregion


        public IEnumerable<Key1> PrimaryKeys => Map.Keys;
        public IEnumerable<Key2> SecondaryKeys => MapInverse.Keys;
    }
}
