namespace G3NexusBackend.DTOs
{
    public class ApiResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
    }
}