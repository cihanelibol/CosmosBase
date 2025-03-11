namespace CosmosBase
{
    public class ApiResponse
    {
        public object? Data { get; set; }
        public bool IsSuccessful { get; set; }
        public string? Message { get; set; }
        public object? Error { get; set; }
        public int StatusCode { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
