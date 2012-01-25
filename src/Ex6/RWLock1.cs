using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex6
{
    public class RWLock
    {
        private bool _writing;
        private int _nReading;

        public RWLock()
        {
            _writing = false;
            _nReading = 0;
        }

        // Acquire read (shared) access
        public void EnterRead()
        {
            lock (this)
            {
                while (_writing)
                {
                    Monitor.Wait(this);
                }
                _nReading++;
            }
        }

        // Acquire write (exclusive) access
        public void EnterWrite()
        {
            lock (this)
            {
                while (_nReading > 0 || _writing)
                {
                    Monitor.Wait(this);
                }
                _writing = true;
            }
        }

        // Release read (shared) access
        public void ExitRead()
        {
            lock (this)
            {
                _nReading--;
                if (_nReading == 0)
                {
                    Monitor.PulseAll(this);
                }
            }
        }

        // Release write (exclusive) access
        public void ExitWrite()
        {
            lock (this)
            {
                _writing = false;
                Monitor.PulseAll(this);
            }
        }
    }
}
