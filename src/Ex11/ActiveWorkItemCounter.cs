using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FileFinder
{
    class ActiveWorkItemCounter
    {
        private Action _action;
        private int _count_active_work_items;

        public ActiveWorkItemCounter(Action action)
        {
            _action = action;
            _count_active_work_items = 1;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _count_active_work_items);
        }

        internal void Decrement()
        {
            if (Interlocked.Decrement(ref _count_active_work_items) == 0)
            {
                _action();
            }
        }
    }
}
