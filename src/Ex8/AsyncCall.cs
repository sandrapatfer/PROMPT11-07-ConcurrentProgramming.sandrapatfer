using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex8
{
    public class FuncAsyncResult<T> : IAsyncResult
    {
        public T TResult { get; set; }
        public Exception ExceptionOut { get; set; }

        #region IAsyncResult Members

        public object AsyncState
        {
            get;
            set;
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get;
            set;
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get;
            set;
        }

        #endregion
    }

    public static class AsyncCall
    {
        public static IAsyncResult BeginCall<TIn, TOut>(this Func<TIn, TOut> func, TIn arg, AsyncCallback callback, Object state)
        {
            var result = new FuncAsyncResult<TOut>()
            {
                AsyncState = state,
                IsCompleted = false,
                AsyncWaitHandle = new ManualResetEvent(false)
            };
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    result.TResult = func(arg);
                    result.IsCompleted = true;
                    var ev = result.AsyncWaitHandle as ManualResetEvent;
                    ev.Set();
                    callback(result);
                }
                catch (Exception e)
                {
                    result.ExceptionOut = e;
                }
            });
            return result;
        }

        public static TOut EndCall<TIn, TOut>(this Func<TIn, TOut> func, IAsyncResult iar)
        {
            var result = iar as FuncAsyncResult<TOut>;
            if (!result.IsCompleted)
            {
                result.AsyncWaitHandle.WaitOne();
            }
            if (result.ExceptionOut != null)
            {
                throw result.ExceptionOut;
            }
            return result.TResult;
        }

    }
}
