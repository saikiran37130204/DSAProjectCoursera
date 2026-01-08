using System;
using System.Diagnostics;
using PriorityQueue; // for heap-based queue
namespace PriorityQueue
{
    class Program
    {
        static void Main()
        {
            const int N = 10000;

            // ---------------- Test Initial Code ----------------
            var sortQueue = new ApiRequestQueue();
            var sw1 = Stopwatch.StartNew();

            for (int i = N; i > 0; i--)
            {
                sortQueue.Enqueue(new ApiRequest("/test", i));
            }

            sw1.Stop();
            Console.WriteLine($"Initial (List.Sort) Enqueue Time: {sw1.ElapsedMilliseconds} ms");

            // ---------------- Test Optimized Code ----------------
            var heapQueue = new ThreadSafeMinHeapPriorityQueue();
            var sw2 = Stopwatch.StartNew();

            for (int i = N; i > 0; i--)
            {
                heapQueue.Enqueue(new ApiRequest("/test", i));
            }

            sw2.Stop();
            Console.WriteLine($"Optimized (Min-Heap) Enqueue Time: {sw2.ElapsedMilliseconds} ms");
        }
    }
}
