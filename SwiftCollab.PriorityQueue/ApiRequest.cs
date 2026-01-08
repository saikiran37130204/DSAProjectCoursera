namespace PriorityQueue
{
    // Simple API request model
    public class ApiRequest
    {
        public string Endpoint { get; set; }
        public int Priority { get; set; }   // Lower value = higher priority

        public ApiRequest(string endpoint, int priority)
        {
            Endpoint = endpoint;
            Priority = priority;
        }
    }
}

