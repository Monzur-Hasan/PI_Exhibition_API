using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.OtherModels.Response
{
    public class ExecutionResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
       
        public static ExecutionResponse<T> SuccessResponse(string message, T data)
        {
            return new ExecutionResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ExecutionResponse<T> FailureResponse(string message, Dictionary<string, string> errors = null, int statusCode = 400)
        {           

            return new ExecutionResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,             
                Errors = errors ?? new Dictionary<string, string>()
            };
        }
        
    }

}
