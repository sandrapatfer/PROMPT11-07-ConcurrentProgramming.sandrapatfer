using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConcurrentCache
{
    public class ConcurrentCache<TKey, TValue>
    {
        private Func<TKey, TValue> _factory;
        private Dictionary<TKey, Future<TValue>> _cache = new Dictionary<TKey, Future<TValue>>();

        public ConcurrentCache(Func<TKey, TValue> factory)
        {
            _factory = factory;
        }

        public Future<TValue> Get(TKey key)
        {
            Future<TValue> cacheValue;
            lock (_cache)
            {
                if (!_cache.ContainsKey(key))
                {
                    cacheValue = new Future<TValue>();
                    _cache.Add(key, cacheValue);
                    ThreadPool.QueueUserWorkItem((o) =>
                        {
                            var value = _factory(key);
                            cacheValue.Value = value;
                        });
                }
                else
                {
                    cacheValue = _cache[key];
                }
            }

            return cacheValue;
        }
    }

    public class Future<TValue>
    {
        private volatile bool _calculating = true;
        private TValue _value;

        public TValue Value
        {
            get
            {
                if (_calculating)
                {
                    lock (this)
                    {
                        if (_calculating)
                        {
                            Monitor.Wait(this);
                        }
                    }
                }
                return _value;
            }
            set
            {
                lock (this)
                {
                    _value = value;
                    _calculating = false;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
