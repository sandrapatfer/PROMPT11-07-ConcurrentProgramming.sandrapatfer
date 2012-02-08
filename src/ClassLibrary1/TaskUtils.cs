using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ClassLibrary1
{
    public static class TaskUtils
    {
        public static Task<T> GetFirstResult<T>(params Task<T>[] tasks)
        {
            var cts = new TaskCompletionSource<T>();
            int nTasks = tasks.Length;

            foreach(var task in tasks)
            {
                task.ContinueWith( (t) =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cts.TrySetResult(t.Result);
                    }
                    else if (Interlocked.Decrement(ref nTasks) == 0)
                    {
                        cts.TrySetException(new Exception("None completed"));
                    }
                });
            }

            return cts.Task;
        }

        public static Task<T> WithTimeout<T>(this Task<T> task, int timeout)
        {
            var cts = new TaskCompletionSource<T>();

            var timer = new Timer((o) => { cts.TrySetException(new TimeoutException()); }, null, timeout, Timeout.Infinite);
            task.ContinueWith((t) =>
            {
                timer.Dispose();
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    cts.TrySetResult(t.Result);
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    cts.TrySetException(t.Exception);
                }
                else //if (t.Status == TaskStatus.Canceled)
                {
                    cts.TrySetCanceled();
                }
            });

            return cts.Task;
        }
    }
}
