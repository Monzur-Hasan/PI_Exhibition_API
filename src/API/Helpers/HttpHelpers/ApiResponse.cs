namespace API.Helpers.HttpHelpers
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(string message, bool success = true)
        {
            Message = message;
            Success = success;
        }

        public ApiResponse(string message, dynamic data, bool success = true)
            : this(message, success)
        {
            Data = data;
        }
    }
}
                   