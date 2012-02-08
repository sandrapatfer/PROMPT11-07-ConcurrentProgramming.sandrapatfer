﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskUtils
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
            //if (task == null) throw new ArgumentNullException("task");

            if (task == null || (task_t = task as Task<TResult>) == null)
            {
                var tcs2 = new TaskCompletionSource<TResult>();
                tcs2.SetResult(default(TResult));
                task_t = tcs2.Task;
            }
            tcs.SetFromTask<TResult>(task_t);
        }
    }
}
