using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Threading;

namespace MoviesService.Utils
{
    public static class TaskUtils
    {
        // Taken from http://social.msdn.microsoft.com/Forums/da-DK/parallelextensions/thread/56f3f9fd-e124-4d62-bb29-de67a9d1d0e8
        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> tcs, Task<TResult> task)
        {
            if (tcs == null) throw new ArgumentNullException("tcs");
            if (task == null) throw new ArgumentNullException("task");

            switch (task.Status)
            {
                case TaskStatus.RanToCompletion: tcs.SetResult(task.Result); break;
                case TaskStatus.Faulted: tcs.SetException(task.Exception.InnerExceptions); break;
                case TaskStatus.Canceled: tcs.SetCanceled(); break;
                default: throw new InvalidOperationException("The task was not completed.");
            }
        }

        public static void SetFromTask<TResult>(this TaskCompletionSource<TResult> tcs, Task task)
        {
            Task<TResult> task_t = null;

            if (tcs == null) throw new ArgumentNullException("tcs");

            if (task == null || (task_t = task as Task<TResult>) == null)
            {
                var tcs2 = new TaskCompletionSource<TResult>();
                tcs2.SetResult(default(TResult));
                task_t = tcs2.Task;
            }
            tcs.SetFromTask<TResult>(task_t);
        }

        public static Task<T> WhenAll<T>(params Task<T>[] tasks)
        {
            var cts = new TaskCompletionSource<T>();
            int nTasks = tasks.Length;
            foreach (var task in tasks)
            {
                task.ContinueWith((t) =>
                {
                    if (Interlocked.Decrement(ref nTasks) == 0)
                    {
                        cts.TrySetResult(t.Result);
                    }
/*                    else if (t.Status == TaskStatus.RanToCompletion)
                    {
                        cts.TrySetException(new Exception("None completed"));
                    }*/
                });
            }

            return null;
        }

        public static Task WhenAll(params Task[] tasks)
        {
            var cts = new TaskCompletionSource<bool>();
            int nTasks = tasks.Length;
            foreach (var task in tasks)
            {
                task.ContinueWith((t) =>
                {
                    // TODO, deal with errors
                    if (Interlocked.Decrement(ref nTasks) == 0)
                    {
                        cts.TrySetResult(true);
                    }
/*                    else (t.Status == TaskStatus.RanToCompletion)
                    {
                        cts.TrySetException(new Exception("None completed"));
                    }*/
                });
            }

            return cts.Task;
        }

        public static Task NewDelayTask(int time)
        {
            var cts = new TaskCompletionSource<object>();
            new Timer((o) =>
            {
                cts.SetResult(null);
            }, null, time, Timeout.Infinite);
            return cts.Task;
        }

    }
}