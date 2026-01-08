using PriorityQueue;
namespace PriorityQueue
{
    // Simple API request model
    
    

    /// <summary>
    /// Thread-safe Min-Heap Priority Queue
    ///
    /// LLM-driven improvements applied:
    /// 1. Replaced List.Sort() with a Binary Min-Heap.
    /// 2. Reduced insertion complexity from O(n log n) to O(log n).
    /// 3. Optimized Dequeue from O(n) to O(log n).
    /// 4. Added bulk enqueue support using heapify.
    /// 5. Added thread safety for concurrent access.
    /// </summary>
    public class ThreadSafeMinHeapPriorityQueue
    {
        // LLM suggestion:
        // Use a List as an array-backed Binary Heap instead of sorting the list each time.
        
        private readonly List<ApiRequest> _heap = new();

        // LLM suggestion:
        // Add locking to ensure thread-safe access when multiple threads enqueue/dequeue.
        private readonly object _lock = new();

        // ---------------- Enqueue (O(log n)) ----------------
        public void Enqueue(ApiRequest request)
        {
            lock (_lock) // Thread safety for concurrent producers
            {
                // LLM improvement:
                // Insert element at the end and restore heap property using SiftUp.
                // This avoids sorting the entire list (previously O(n log n)).
                _heap.Add(request);
                SiftUp(_heap.Count - 1);
            }
        }

        // ---------------- Bulk Enqueue (O(n + m)) ----------------
        public void EnqueueBatch(IEnumerable<ApiRequest> requests)
        {
            lock (_lock) // Thread safety for batch insert
            {
                // LLM improvement:
                // Add all items first instead of inserting one by one.
                _heap.AddRange(requests);

                // LLM improvement:
                // Use Floyd’s heap construction (heapify) to restore heap order.
                // This runs in O(n + m), which is more efficient than m * O(log n).
                for (int i = (_heap.Count / 2) - 1; i >= 0; i--)
                {
                    SiftDown(i);
                }
            }
        }

        // ---------------- Dequeue (O(log n)) ----------------
        public ApiRequest? Dequeue()
        {
            lock (_lock) // Thread safety for concurrent consumers
            {
                if (_heap.Count == 0) return null;

                // LLM improvement:
                // Always remove the root (minimum priority element).
                ApiRequest root = _heap[0];

                // Move last element to root and restore heap property.
                ApiRequest last = _heap[^1];
                _heap.RemoveAt(_heap.Count - 1);

                if (_heap.Count > 0)
                {
                    _heap[0] = last;

                    // LLM improvement:
                    // SiftDown restores heap order in O(log n),
                    // replacing the earlier O(n) RemoveAt(0) shifting.
                    SiftDown(0);
                }

                return root;
            }
        }

        // ---------------- Heap Helpers ----------------

        // LLM improvement:
        // SiftUp restores heap order after insertion in O(log n).
        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;

                // Stop if heap property is satisfied
                if (_heap[index].Priority >= _heap[parent].Priority)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        // LLM improvement:
        // SiftDown restores heap order after removal in O(log n).
        private void SiftDown(int index)
        {
            int count = _heap.Count;

            while (true)
            {
                int left = 2 * index + 1;
                int right = left + 1;
                int smallest = index;

                if (left < count && _heap[left].Priority < _heap[smallest].Priority)
                    smallest = left;

                if (right < count && _heap[right].Priority < _heap[smallest].Priority)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        // Swap helper for heap operations
        private void Swap(int i, int j)
        {
            ApiRequest temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }
    }
}
