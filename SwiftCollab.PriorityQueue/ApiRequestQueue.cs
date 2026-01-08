using System.Collections.Generic; 
using PriorityQueue;
namespace PriorityQueue
{
    // Simple API request model
   
    public class ApiRequestQueue
    {
        
        private List<ApiRequest> requests = new List<ApiRequest>();
        public void Enqueue(ApiRequest request)
        {
            requests.Add(request);
            requests.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // Inefficient sorting
        }
        public ApiRequest Dequeue()
        {
            if (requests.Count == 0)
                return null;
            ApiRequest nextRequest = requests[0];
            requests.RemoveAt(0);
            return nextRequest;
        }
    }
}