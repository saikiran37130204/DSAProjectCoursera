using System;
using System.Collections.Generic;

namespace SwiftCollab.TaskTreeOptimzer
{
    // ================================
    // OPTIMIZED AVL TREE
    // ================================
    public class AVLTree
    {
        public class Node
        {
            public int Value;
            public Node? Left, Right;
            public int Height;

            public Node(int value)
            {
                Value = value;
                Height = 1;
            }
        }

        public Node? Root { get; private set; }
        public int Count { get; private set; } = 0;

        // Iterative AVL Insert
        public bool Insert(int value)
        {
            bool inserted = false;
            Root = InsertWithPath(Root, value, ref inserted);
            if (inserted) Count++;
            return inserted;
        }

        public bool Contains(int value)
        {
            Node? cur = Root;
            while (cur != null)
            {
                if (value == cur.Value) return true;
                cur = value < cur.Value ? cur.Left : cur.Right;
            }
            return false;
        }

        private Node InsertWithPath(Node? root, int value, ref bool inserted)
        {
            if (root == null)
            {
                inserted = true;
                return new Node(value);
            }

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
                    return root; // duplicate
                }
            }

            while (path.Count > 0)
            {
                var (node, nodeParent) = path.Pop();
                UpdateHeight(node);

                int balance = GetBalance(node);
                Node newRoot = node;

                if (balance > 1)
                {
                    if (GetBalance(node.Left) >= 0)
                        newRoot = RotateRight(node);
                    else
                    {
                        node.Left = RotateLeft(node.Left!);
                        newRoot = RotateRight(node);
                    }
                }
                else if (balance < -1)
                {
                    if (GetBalance(node.Right) <= 0)
                        newRoot = RotateLeft(node);
                    else
                    {
                        node.Right = RotateRight(node.Right!);
                        newRoot = RotateLeft(node);
                    }
                }

                if (nodeParent == null)
                    root = newRoot;
                else if (nodeParent.Left == node)
                    nodeParent.Left = newRoot;
                else
                    nodeParent.Right = newRoot;
            }

            return root;
        }

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
    }
}
