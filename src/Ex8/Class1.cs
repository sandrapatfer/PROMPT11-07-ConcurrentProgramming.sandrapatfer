using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ex8
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<int, bool> v1 = (arg0) =>
            {
                Console.WriteLine("In func, arg: {0}, id: {1}", arg0, Thread.CurrentThread.ManagedThreadId);
                return true;
            };
            v1.BeginCall(20, (r) =>
            {
                var result = v1.EndCall(r);
                Console.WriteLine("Result: {0}, id: {1}", result, Thread.CurrentThread.ManagedThreadId);
            }, null);
            v1.BeginInvoke(30, (r) =>
            {
                var result = v1.EndInvoke(r);
                Console.WriteLine("Result: {0}, id: {1}", result, Thread.CurrentThread.ManagedThreadId);
            }, null);
            Console.ReadKey();
        }
    }
}