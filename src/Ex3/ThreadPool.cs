using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex3
{
    public delegate void Proc();

    public class ThreadPool
    {
        private int _min;
        private int _max;
        private int _current;
        private int _free;
        private readonly LinkedList<Proc> _procList = new LinkedList<Proc>();
 
        public ThreadPool(int min, int max)
        {
            if (min < 1)
            {
                min = 1;
            }
            _min = min;
            _max = max;
            _current = _min;
            _free = _min;
            for (int i = 0; i < _min; i++)
            {
                (new Thread(ThreadWork)).Start();
            }
        }

        public void QueueProc(Proc proc)
        {
            lock (_procList)
            {
                _procList.AddLast(proc);
                Monitor.Pulse(_procList);
            }
        }

        public void ThreadWork()
        {
            lock (_procList)
            {
                --_free;
            }
            while (true)
            {
                Proc proc;
                lock (_procList)
                {
                    _free++;
                    if (_current > _min && _free > (_procList.Count + 1))
                    {
                        _current--;
                        return;
                    }

                    while (_procList.Count == 0)
                    {
                        Monitor.Wait(_procList);
                    }
                    proc = _procList.First.Value;
                    _procList.RemoveFirst();
                    --_free;
                    
                    if (_current < _max && _free == 0)
                    {
                        (new Thread(ThreadWork)).Start();
                        _current++;
                        _free++;
                    }
                }
                proc();
            }
        }
    }
}
