using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex6
{
    // Problema: concluida uma escrita, todos os leitores que se encontram em fila de espera são desbloqueados
    // Este não funciona, a solução é o RWLockEx

    public class RWLock3
    {
        private bool _writing;
        private int _nReading;
        private int _nWritersWaiting;
        private int _readerGen;

        public RWLock3()
        {
            _writing = false;
            _nReading = 0;
            _nWritersWaiting = 0;
            _readerGen = 0;
        }

        // Acquire read (shared) access
        public void EnterRead()
        {
            lock (this)
            {
                int myGen = _readerGen;
                while (_writing || myGen == _readerGen)
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
                _nWritersWaiting++;
                try
                {
                    while (_nReading > 0 || _writing || _nReadersNewWrite > 0)
                    {
                        Monitor.Wait(this);
                    }
                }
                catch(ThreadInterruptedException)
                {
                    if (!_writing && (_nWritersWaiting == 1 || _nReading == 0))
                    {
                        Monitor.PulseAll(this);
                    }
                    throw;
                }
                finally
                {
                    _nWritersWaiting--;
                }
                _writing = true;
            }
        }

        // Release read (shared) access
        public void ExitRead()
        {
            ThreadInterruptedException interrupted = null;
            while (true)
            {
                try
                {
                    Monitor.Enter(this);
                    break;
                }
                catch (ThreadInterruptedException e)
                {
                    interrupted = e;
                }
            }

            try
            {
                _nReading--;
                if (_nReading == 0)
                {
                    Monitor.PulseAll(this);
                }
            }
            finally
            {
                Monitor.Exit(this);
            }

            if (interrupted != null)
            {
                throw interrupted;
            }
        }

        // Release write (exclusive) access
        public void ExitWrite()
        {
            ThreadInterruptedException interrupted = null;
            while (true)
            {
                try
                {
                    Monitor.Enter(this);
                    break;
                }
                catch (ThreadInterruptedException e)
                {
                    interrupted = e;
                }
            }

            try
            {
                _writing = false;
                _readerGen++;
                Monitor.PulseAll(this);
            }
            finally
            {
                Monitor.Exit(this);
            }

            if (interrupted != null)
            {
                throw interrupted;
            }
        }
    }
}
