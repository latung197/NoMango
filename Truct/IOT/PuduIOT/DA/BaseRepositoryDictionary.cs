namespace PuduIOT.DA
{
    public abstract class BaseRepositoryDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _repoDictionary = new();

        protected readonly object _sync = new();
        public Dictionary<TKey, TValue> GetRepository()
        {
            lock (_sync)
            {
                return _repoDictionary.ToDictionary(k => k.Key, v => v.Value);
            }
        }
        public void RemoveKey(TKey key)
        {
            lock (_sync)
            {
                if (_repoDictionary.ContainsKey(key))
                {
                    _repoDictionary.Remove(key);
                }
            }
        }
        public bool ContainsKey(TKey key)
        {
            lock (_sync)
            {
                return _repoDictionary.ContainsKey(key);
            }
        }

        public TValue? GetValueByKey(TKey key)
        {
            lock (_sync)
            {
                return _repoDictionary.ContainsKey(key) ? _repoDictionary[key] : default;
            }
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            lock (_sync)
            {
                _repoDictionary[key] = value;
            }
        }

        public int Count()
        {
            lock (_sync)
            {
                return _repoDictionary.Count;
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection GetKeys()
        {
            lock (_sync)
            {
                return _repoDictionary.Keys;
            }
        }
        /// <summary>
        /// Get all values - data can be change
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, TValue>.ValueCollection GetValues()
        {
            lock (_sync)
            {
                return _repoDictionary.Values;
            }
        }
        /// <summary>
        /// Use for readonly list - no change data at all
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<TKey> GetListKeys()
        {
            lock (_sync)
            {
                return _repoDictionary.Keys.ToList().AsReadOnly();
            }
        }

        public IReadOnlyList<TValue> GetListValues()
        {
            lock (_sync)
            {
                return _repoDictionary.Values.ToList().AsReadOnly();
            }
        }
    }
}
