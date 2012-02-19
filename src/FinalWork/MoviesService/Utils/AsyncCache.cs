using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace MoviesService.Utils
{
    public class AsyncCache<TKey, TValue>
    {
        private Func<TKey, Task<TValue>> _valueFactory;
        private ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _dictionary;

        public AsyncCache(Func<TKey, Task<TValue>> valueFactory, IEqualityComparer<TKey> comparer)
        {
            _valueFactory = valueFactory;
            _dictionary = new ConcurrentDictionary<TKey,Lazy<Task<TValue>>>(comparer);
        }

        public Task<TValue> Get(TKey key)
        {
            return _dictionary.GetOrAdd(key, keyToAdd => 
                new Lazy<Task<TValue>>(() => 
                    _valueFactory(keyToAdd))).Value;
        }
    }
}