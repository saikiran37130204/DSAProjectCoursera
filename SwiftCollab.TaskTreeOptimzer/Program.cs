using System;
using System.Diagnostics;

namespace SwiftCollab.TaskTreeOptimzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int n = 10_000;
            int[] data = new int[n];
            Random rng = new Random(12345);

            // Pre-generate data (fair benchmark)
            for (int i = 0; i < n; i++)
                data[i] = rng.Next();

            Console.WriteLine("=== PERFORMANCE COMPARISON ===\n");

            // -----------------------------
            // Initial Unbalanced BST
            // -----------------------------
            var bst = new BST();
            var sw1 = Stopwatch.StartNew();
            foreach (var v in data)
                bst.Insert(v);
            sw1.Stop();

           Console.WriteLine($"Initial BST Insert Time: {sw1.ElapsedMilliseconds} ms");

            // -----------------------------
            // Optimized AVL Tree
            // -----------------------------
            var avl = new AVLTree();
            var sw2 = Stopwatch.StartNew();
            foreach (var v in data)
                avl.Insert(v);
            sw2.Stop();

            Console.WriteLine($"Optimized AVL Insert Time: {sw2.ElapsedMilliseconds} ms");

            Console.WriteLine("\n=== WORST-CASE TEST (SORTED INPUT) ===");

            // // Worst-case input for BST
            var bstWorst = new BST();
            var sw3 = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
                bstWorst.Insert(i);
            sw3.Stop();

            Console.WriteLine($"Initial BST (sorted input): {sw3.ElapsedMilliseconds} ms");

            // Worst-case input for AVL (still balanced)
            var avlWorst = new AVLTree();
            var sw4 = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
                avlWorst.Insert(i);
            sw4.Stop();

            Console.WriteLine($"Optimized AVL (sorted input): {sw4.ElapsedMilliseconds} ms");

            Console.WriteLine("\nDone.");
        }
    }
}
