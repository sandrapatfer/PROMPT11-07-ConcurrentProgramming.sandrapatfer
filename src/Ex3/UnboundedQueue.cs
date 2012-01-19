using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex3
{
    class UnboundedQueue<T>
    {
        private readonly LinkedList<T> items = new LinkedList<T>();
        public T Get()
        {
            lock (items)
            {
                while (items.Count == 0)
                {
                    Monitor.Wait(items);
                }
                T res = items.First.Value;
                items.RemoveFirst();
                return res;
            }
        }
        public void Put(T val)
        {
            lock (items)
            {
                items.AddLast(val);
                Monitor.Pulse(items);
            }
        }
    }
}
