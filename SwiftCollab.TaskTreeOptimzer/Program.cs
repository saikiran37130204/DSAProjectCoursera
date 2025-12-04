using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace SwiftCollab.TaskTreeOptimzer
{
    internal class Program
    {
        // ================================
        // NODE CLASS (AVL Tree Node)
        // ================================
        public class Node
        {
            // Node holds a value (priority for tasks)
            public int Value;
            public Node? Left { get; set; }
            public Node? Right { get; set; }

            // Height is tracked for AVL balancing
            public int Height { get; set; }

            public Node(int value)
            {
                Value = value;
                Height = 1;
            }
        }

        // ============================================
        // BALANCED BINARY TREE (AVL TREE IMPLEMENTATION)
        // ============================================
        public class BalancedBinaryTree
        {
            public Node? Root { get; private set; }
            public int Count { get; private set; } = 0;

            // INSERT (Iterative with AVL balancing)
            // LLM Improvement:
            //   - Avoid recursion for insert (better performance & stack safety)
            //   - Maintain AVL balance bottom-up using stored path
            public bool Insert(int value)
            {
                bool inserted = false;
                Root = InsertWithPath(Root, value, ref inserted);
                if (inserted) Count++;
                return inserted;
            }

            // Iterative Search (non-recursive)
            // LLM Improvement:
            //   - Converted recursive search → iterative to reduce overhead
            public bool Contains(int value)
            {
                Node? node = Root;
                while (node != null)
                {
                    if (value == node.Value) return true;
                    node = value < node.Value ? node.Left : node.Right;
                }
                return false;
            }

            // Iterative insert using path stack + bottom-up AVL rebalancing
            private Node InsertWithPath(Node? root, int value, ref bool inserted)
            {
                if (root == null)
                {
                    inserted = true;
                    return new Node(value);
                }

                // Stack stores both node & its parent for safe reconnecting after rotations
                var path = new Stack<(Node node, Node? parent)>();
                Node current = root;
                Node? parent = null;

                while (true)
                {
                    path.Push((current, parent));

                    if (value < current.Value)
                    {
                        if (current.Left == null)
                        {
                            current.Left = new Node(value);
                            path.Push((current.Left, current));
                            inserted = true;
                            break;
                        }
                        parent = current;
                        current = current.Left;
                    }
                    else if (value > current.Value)
                    {
                        if (current.Right == null)
                        {
                            current.Right = new Node(value);
                            path.Push((current.Right, current));
                            inserted = true;
                            break;
                        }
                        parent = current;
                        current = current.Right;
                    }
                    else
                    {
                        // Duplicate → ignore
                        return root;
                    }
                }

                // Bottom-up rebalancing using the stored path
                while (path.Count > 0)
                {
                    var (node, nodeParent) = path.Pop();
                    UpdateHeight(node);

                    int balance = GetBalance(node);
                    Node newRoot = node;

                    // Standard AVL balancing cases
                    if (balance > 1)
                    {
                        if (GetBalance(node.Left) >= 0)         // Left-Left
                            newRoot = RotateRight(node);
                        else                                    // Left-Right
                        {
                            node.Left = RotateLeft(node.Left!);
                            newRoot = RotateRight(node);
                        }
                    }
                    else if (balance < -1)
                    {
                        if (GetBalance(node.Right) <= 0)        // Right-Right
                            newRoot = RotateLeft(node);
                        else                                    // Right-Left
                        {
                            node.Right = RotateRight(node.Right!);
                            newRoot = RotateLeft(node);
                        }
                    }

                    // Reattach new subtree root to parent
                    if (nodeParent == null)
                    {
                        root = newRoot; // update real tree root
                    }
                    else if (nodeParent.Left == node)
                    {
                        nodeParent.Left = newRoot;
                    }
                    else
                    {
                        nodeParent.Right = newRoot;
                    }
                }

                return root;
            }

            // REMOVE (Recursive but safe for AVL because height is small)
            // LLM Improvement:
            //   - Added full remove functionality
            //   - Handles leaf, one-child, and two-child deletion cases
            //   - Maintains AVL balance after deletion
            public bool Remove(int value)
            {
                Root = RemoveRec(Root, value, out bool removed);
                if (removed) Count--;
                return removed;
            }

            private Node? RemoveRec(Node? node, int value, out bool removed)
            {
                removed = false;
                if (node == null) return null;

                if (value < node.Value)
                    node.Left = RemoveRec(node.Left, value, out removed);
                else if (value > node.Value)
                    node.Right = RemoveRec(node.Right, value, out removed);
                else
                {
                    removed = true;

                    // One-child or no-child case
                    if (node.Left == null || node.Right == null)
                        return node.Left ?? node.Right;

                    // Two-child case: get inorder successor
                    Node successor = node.Right;
                    while (successor.Left != null)
                        successor = successor.Left;

                    node.Value = successor.Value;
                    node.Right = RemoveRec(node.Right, successor.Value, out _);
                }

                // Update height after delete
                UpdateHeight(node);

                int balance = GetBalance(node);

                // Rebalance after remove
                if (balance > 1)
                {
                    if (GetBalance(node.Left) >= 0)
                        return RotateRight(node);
                    node.Left = RotateLeft(node.Left!);
                    return RotateRight(node);
                }
                if (balance < -1)
                {
                    if (GetBalance(node.Right) <= 0)
                        return RotateLeft(node);
                    node.Right = RotateRight(node.Right!);
                    return RotateLeft(node);
                }

                return node;
            }

            // Change Priority = Remove old value + Insert new value
            // LLM Improvement: Simple & correct for AVL
            public bool ChangePriority(int oldValue, int newValue)
            {
                if (!Contains(oldValue)) return false;
                Remove(oldValue);
                Insert(newValue);
                return true;
            }

            // Utility functions for AVL rotations and balancing
            private int GetHeight(Node? n) => n?.Height ?? 0;
            private void UpdateHeight(Node n) =>
                n.Height = 1 + Math.Max(GetHeight(n.Left), GetHeight(n.Right));
            private int GetBalance(Node? n) =>
                n == null ? 0 : GetHeight(n.Left) - GetHeight(n.Right);

            private Node RotateRight(Node y)
            {
                Node x = y.Left!;
                Node? T2 = x.Right;
                x.Right = y;
                y.Left = T2;
                UpdateHeight(y);
                UpdateHeight(x);
                return x;
            }

            private Node RotateLeft(Node x)
            {
                Node y = x.Right!;
                Node? T2 = y.Left;
                y.Left = x;
                x.Right = T2;
                UpdateHeight(x);
                UpdateHeight(y);
                return y;
            }

            // Iterative In-order Traversal
            // LLM Improvement: avoids recursion to handle large datasets safely
            public void PrintInOrderIterative()
            {
                var stack = new Stack<Node>();
                Node? cur = Root;

                while (stack.Count > 0 || cur != null)
                {
                    while (cur != null)
                    {
                        stack.Push(cur);
                        cur = cur.Left;
                    }
                    cur = stack.Pop();
                    Console.Write(cur.Value + " ");
                    cur = cur.Right;
                }
                Console.WriteLine();
            }
        }

        // =====================================================
        // MAIN METHOD — Benchmark + Validation
        // =====================================================
        static void Main(string[] args)
        {
            var tree = new BalancedBinaryTree();
            var rng = new Random(12345);

            int n = 100000;
            int knownValue = -1;

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < n; i++)
            {
                int v = rng.Next();
                if (i == n / 2) knownValue = v;
                tree.Insert(v);
            }
            sw.Stop();

            Console.WriteLine($"Inserted {tree.Count} unique nodes in {sw.ElapsedMilliseconds} ms");

            // Validation tests
            Console.WriteLine($"Contains known value? {tree.Contains(knownValue)}");
            tree.ChangePriority(knownValue, -1234);
            Console.WriteLine($"Contains updated value? {tree.Contains(-1234)}");
            tree.Remove(-1234);
            Console.WriteLine($"Contains removed value? {tree.Contains(-1234)}");

            Console.WriteLine("\nDone.");
        }
    }
}
